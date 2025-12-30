using Godot;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

public static class StageSaveService {
    private const string SaveFilePath = "user://save.dat";

    private static readonly JsonSerializerOptions JsonOptions = new() {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private static readonly object SyncRoot = new();
    private static StageSavePayload _data = new();
    private static bool _loaded;

    // Tujuan: pastikan data save sudah dibaca sekali sebelum dipakai service lain.
    public static void EnsureLoaded() {
        if (_loaded) return;

        lock (SyncRoot) {
            if (_loaded) return;

            LoadInternal();
            _loaded = true;
        }
    }

    // Tujuan: ambil catatan progres untuk stage tertentu.
    // Input: stageId berisi kunci stage yang mau dicek.
    // Output: ngembaliin salinan StageSaveEntry supaya pemanggil gak ubah data asli.
    public static StageSaveEntry GetRecord(string stageId) {
        EnsureLoaded();

        lock (SyncRoot) {
            stageId = NormalizeStageId(stageId);
            if (!_data.Stages.TryGetValue(stageId, out var entry)) {
                entry = new StageSaveEntry();
                _data.Stages[stageId] = entry;
            }

            return entry.Clone();
        }
    }

    // Tujuan: register penyelesaian stage sekaligus update waktu terbaik bila lebih cepat.
    // Input: stageId buat identitas level dan elapsedSeconds buat lama permainan.
    // Output: balikin StageCompletionUpdate berisi best time terbaru dan flag rekor baru.
    public static StageCompletionUpdate RegisterCompletion(string stageId, double elapsedSeconds) {
        EnsureLoaded();

        lock (SyncRoot) {
            stageId = NormalizeStageId(stageId);
            if (!_data.Stages.TryGetValue(stageId, out var entry)) {
                entry = new StageSaveEntry();
                _data.Stages[stageId] = entry;
            }

            bool isNewRecord = !entry.Completed || !entry.BestTimeSeconds.HasValue || elapsedSeconds < entry.BestTimeSeconds.Value;
            entry.Completed = true;
            if (isNewRecord) entry.BestTimeSeconds = elapsedSeconds;

            SaveInternal();
            return new StageCompletionUpdate(entry.BestTimeSeconds, isNewRecord);
        }
    }

    // Tujuan: helper yang nyalurin request update best time ke RegisterCompletion.
    // Input: stageId dan elapsedSeconds sama seperti fungsi utama.
    // Output: balikin StageCompletionUpdate yang sama dari RegisterCompletion.
    public static StageCompletionUpdate UpdateBestTimeIfFaster(string stageId, double elapsedSeconds) {
        return RegisterCompletion(stageId, elapsedSeconds);
    }

    // Tujuan: kasih snapshot semua progres stage yang tersimpan.
    // Output: ngembaliin dictionary baru biar aman dari mutasi luar.
    public static IReadOnlyDictionary<string, StageSaveEntry> GetAllRecords() {
        EnsureLoaded();

        lock (SyncRoot) {
            var snapshot = new Dictionary<string, StageSaveEntry>(_data.Stages.Count);
            foreach (var pair in _data.Stages) snapshot[pair.Key] = pair.Value?.Clone() ?? new StageSaveEntry();
            return snapshot;
        }
    }

    // Tujuan: hapus seluruh progres dan mulai dari data kosong.
    public static void Reset() {
        lock (SyncRoot) {
            _data = new StageSavePayload();
            _loaded = true;
            SaveInternal();
        }
    }

    // Tujuan: baca file save dari disk dan muat ke memori.
    private static void LoadInternal() {
        try {
            if (!FileAccess.FileExists(SaveFilePath)) {
                _data = new StageSavePayload();
                return;
            }

            using var file = FileAccess.Open(SaveFilePath, FileAccess.ModeFlags.Read);
            if (file == null) {
                GD.PrintErr("[StageSaveService] Gagal membuka file save.");
                _data = new StageSavePayload();
                return;
            }

            string raw = file.GetAsText();
            if (string.IsNullOrWhiteSpace(raw)) {
                _data = new StageSavePayload();
                return;
            }

            _data = JsonSerializer.Deserialize<StageSavePayload>(raw, JsonOptions) ?? new StageSavePayload();
        }
        catch (Exception ex) {
            GD.PrintErr($"[StageSaveService] Gagal memuat data save: {ex.Message}");
            _data = new StageSavePayload();
        }
    }

    // Tujuan: simpan snapshot progres terbaru ke file user.
    private static void SaveInternal() {
        try {
            string raw = JsonSerializer.Serialize(_data, JsonOptions);
            using var file = FileAccess.Open(SaveFilePath, FileAccess.ModeFlags.Write);
            if (file == null) {
                GD.PrintErr("[StageSaveService] Gagal membuka file save untuk menulis.");
                return;
            }

            file.StoreString(raw);
        }
        catch (Exception ex) {
            GD.PrintErr($"[StageSaveService] Gagal menyimpan data save: {ex.Message}");
        }
    }

    // Tujuan: normalkan id stage biar konsisten saat jadi kunci dictionary.
    // Input: stageId bebas dari pemanggil.
    // Output: string id lowercase tanpa spasi ekstra.
    private static string NormalizeStageId(string stageId) {
        if (string.IsNullOrWhiteSpace(stageId)) return "stage";

        return stageId.Trim().ToLowerInvariant();
    }

    private sealed class StageSavePayload {
        [JsonPropertyName("stages")]
        public Dictionary<string, StageSaveEntry> Stages { get; set; } = new();
    }
}

public sealed class StageSaveEntry {
    [JsonPropertyName("best_time")]
    public double? BestTimeSeconds { get; set; }

    [JsonPropertyName("completed")]
    public bool Completed { get; set; }

    // Tujuan: bikin salinan sederhana supaya data internal tetap aman.
    // Output: ngembaliin StageSaveEntry baru dengan nilai sama.
    public StageSaveEntry Clone() {
        return new StageSaveEntry {
            BestTimeSeconds = BestTimeSeconds,
            Completed = Completed
        };
    }
}

public readonly struct StageCompletionUpdate {
    // Tujuan: nyimpan hasil update setelah stage selesai dimainkan.
    // Input: bestTimeSeconds berisi catatan waktu terbaru, isNewRecord nandain rekor baru atau tidak.
    public StageCompletionUpdate(double? bestTimeSeconds, bool isNewRecord) {
        BestTimeSeconds = bestTimeSeconds;
        IsNewRecord = isNewRecord;
    }

    public double? BestTimeSeconds { get; }
    public bool IsNewRecord { get; }
}
