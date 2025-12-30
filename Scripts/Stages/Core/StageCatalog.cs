using System.Collections.Generic;

public static class StageCatalog {
    public sealed class StageInfo {
        public string Id { get; init; }
        public string DisplayName { get; init; }
        public string ScenePath { get; init; }
        public string Difficulty { get; init; }
        public string GuideSummary { get; init; }
        public IReadOnlyList<string> Tips { get; init; }
    }

    private static readonly StageInfo[] _challengeStages = {
        new StageInfo {
            Id = "rocket",
            DisplayName = "Rocket Stage",
            ScenePath = "res://Scenes/Karya/DynamicPatternStage.tscn",
            Difficulty = "MEDIUM",
            GuideSummary = "Bangun badan roket berskala 0.7; fokuskan segitiga di nose cone dan trapesium untuk sayap samping.",
            Tips = new[] {
                "Klik shape pada palet untuk menaruhnya di area kerja, kemudian drag ke outline abu-abu.",
                "Gunakan tombol Q/E atau ikon rotasi untuk memutar bentuk agar mengikuti outline.",
                "Klik kanan untuk menghapus shape jika salah penempatan."
            }
        },
        new StageInfo {
            Id = "ufo",
            DisplayName = "UFO Stage",
            ScenePath = "res://Scenes/Karya/DynamicPatternStage.tscn",
            Difficulty = "EASY",
            GuideSummary = "Jaga simetri kiri-kanan UFO dan gunakan jajar genjang untuk membentuk cawan serta sayap.",
            Tips = new[] {
                "Mulai dari bagian tengah agar simetris lalu lanjutkan ke sayap kiri dan kanan.",
                "Rotasi 60° sering dipakai untuk menyesuaikan jajar genjang di outline.",
                "Jika shape habis, hapus salah satu di kanvas untuk mengembalikannya ke palet."
            }
        },
        new StageInfo {
            Id = "astronaut",
            DisplayName = "Astronaut Stage",
            ScenePath = "res://Scenes/Karya/DynamicPatternStage.tscn",
            Difficulty = "HARD",
            GuideSummary = "Detailkan astronot skala 0.4; susun panel persegi kecil untuk torso dan helm, sisipkan segitiga untuk aksesori.",
            Tips = new[] {
                "Kerjakan dari torso ke anggota tubuh agar outline mudah diikuti.",
                "Tekan dan tahan scroll untuk pan viewport saat detail berada di sudut layar.",
                "Gunakan zoom (scroll) untuk presisi tinggi ketika menempatkan persegi kecil."
            }
        }
    };

    public static IReadOnlyList<StageInfo> ChallengeStages => _challengeStages;

    // Tujuan: mengembalikan metadata stage berdasarkan ID yang diminta.
    // Input: stageId berupa string ID stage.
    // Output: StageInfo bila ditemukan, null jika tidak ada.
    public static StageInfo GetStageInfo(string stageId) {
        if (string.IsNullOrWhiteSpace(stageId)) return null;

        stageId = stageId.Trim().ToLowerInvariant();
        foreach (var info in _challengeStages) {
            if (info.Id == stageId) return info;
        }

        return null;
    }

    // Tujuan: mencari stage berikutnya setelah ID tertentu untuk navigasi stage.
    // Input: stageId saat ini.
    // Output: StageInfo stage selanjutnya atau null bila sudah terakhir.
    public static StageInfo GetNextStage(string stageId) {
        if (string.IsNullOrWhiteSpace(stageId)) return null;

        stageId = stageId.Trim().ToLowerInvariant();
        for (int i = 0; i < _challengeStages.Length; i++) {
            if (_challengeStages[i].Id == stageId) {
                int nextIndex = i + 1;
                if (nextIndex < _challengeStages.Length) return _challengeStages[nextIndex];
                break;
            }
        }

        return null;
    }
}
