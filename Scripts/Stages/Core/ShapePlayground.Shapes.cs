using Godot;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Matrix4x4 = System.Numerics.Matrix4x4;
using ShapeType = PatternShapeLibrary.ShapeType;

public partial class ShapePlayground {
    // Tujuan: ngatur faktor skala global untuk semua shape aktif.
    // Input: scale angka skala baru yang diminta pengguna.
    protected void SetGlobalShapeScale(float scale) {
        float clamped = Mathf.Max(0.01f, scale);
        if (Mathf.IsEqualApprox(clamped, _globalShapeScale)) return;

        _globalShapeScale = clamped;
        QueueRedraw();
    }

    // Tujuan: skala seluruh shape aktif relatif ke satu titik jangkar.
    // Input: anchor pusat scaling dan ratio faktor pengali jarak.
    protected void ScaleActiveShapes(Vector2 anchor, float ratio) {
        if (_activeShapes.Count == 0) return;
        if (Mathf.IsEqualApprox(ratio, 1f)) return;

        foreach (var shape in _activeShapes) {
            Vector2 offset = shape.WorldPivot - anchor;
            shape.WorldPivot = anchor + offset * ratio;
            OnShapeTransformUpdated(shape, false);
        }

        QueueRedraw();
    }

    // Tujuan: hook yang dijalankan saat shape baru dibuat dari palet.
    // Input: instance adalah ShapeInstance yang baru aktif.
    protected virtual void OnShapeSpawned(ShapeInstance instance) { }

    // Tujuan: hook saat transform shape berubah agar turunan bisa merespon.
    // Input: instance shape yang diubah dan isFinal nandain perubahan permanen.
    protected virtual void OnShapeTransformUpdated(ShapeInstance instance, bool isFinal) { }

    // Tujuan: hook saat shape dihapus supaya turunan bisa bersih-bersih state.
    // Input: instance shape yang baru dihapus dari panggung.
    protected virtual void OnShapeRemoved(ShapeInstance instance) { }

    // Tujuan: hapus sebuah shape dan optionally balikin kuotanya ke palet.
    // Input: shape target yang mau dihapus dan refundPalette nandain kuota dikembalikan.
    private void RemoveShape(ShapeInstance shape, bool refundPalette) {
        if (!_activeShapes.Remove(shape)) return;

        if (refundPalette && shape.PaletteIndex >= 0 && shape.PaletteIndex < _palette.Count) {
            var entry = _palette[shape.PaletteIndex];
            if (!entry.IsUnlimited) {
                entry.Remaining = Mathf.Clamp(entry.Remaining + 1, 0, entry.Quota.MaxCopies);
                _palette[shape.PaletteIndex] = entry;
            }
        }

        OnShapeRemoved(shape);

        if (_selectedShape == shape) _selectedShape = null;
        if (_draggedShape == shape) {
            _draggedShape = null;
            _dragOffset = Vector2.Zero;
        }
    }

    // Tujuan: bersihkan semua shape aktif sekaligus dan atur ulang selection.
    // Input: refundPalette nandain apakah kuota perlu dikembalikan ke palet.
    protected void RemoveAllShapes(bool refundPalette) {
        for (int i = _activeShapes.Count - 1; i >= 0; i--) RemoveShape(_activeShapes[i], refundPalette);
        _selectedShape = null;
        _draggedShape = null;
        _dragOffset = Vector2.Zero;
        QueueRedraw();
    }

    // Tujuan: buat instance shape baru lengkap dengan kuota dan transform awal.
    // Input: type tipe shape, pivot titik dunia, rotationRadians rotasi awal, consumeQuota apakah kuota berkurang.
    // Output: ngembaliin ShapeInstance aktif atau null kalau kuota/palet gak tersedia.
    protected ShapeInstance SpawnShapeInstance(ShapeType type, Vector2 pivot, float rotationRadians, bool consumeQuota) {
        for (int i = 0; i < _palette.Count; i++) {
            var entry = _palette[i];
            if (entry.Quota.Type != type) continue;

            if (consumeQuota && !entry.IsUnlimited && entry.Remaining <= 0) {
                GD.PrintErr($"[ShapePlayground] Kuota untuk {type} habis.");
                return null;
            }

            if (consumeQuota && !entry.IsUnlimited) {
                entry.Remaining = Mathf.Max(0, entry.Remaining - 1);
                _palette[i] = entry;
            }

            var instance = new ShapeInstance {
                Definition = entry.Shape,
                WorldPivot = pivot,
                Rotation = rotationRadians,
                PaletteIndex = i,
                ShapeType = type,
                IsLocked = false
            };

            _activeShapes.Add(instance);
            _selectedShape = instance;
            OnShapeSpawned(instance);
            OnShapeTransformUpdated(instance, false);
            QueueRedraw();
            return instance;
        }

        GD.PrintErr($"[ShapePlayground] Shape {type} tidak tersedia di palet.");
        return null;
    }

    // Tujuan: cek apakah titik tertentu berada di dalam bounding box shape.
    // Input: point koordinat yang mau dites dan shape target.
    // Output: balik true kalau titik jatuh di dalam bounding box kasar shape.
    private bool IsPointInsideShape(Vector2 point, ShapeInstance shape) {
        var points = BuildTransformedPoints(shape);
        if (points.Count == 0) return false;

        float minX = points.Min(p => p.X) - 6f;
        float maxX = points.Max(p => p.X) + 6f;
        float minY = points.Min(p => p.Y) - 6f;
        float maxY = points.Max(p => p.Y) + 6f;

        return point.X >= minX && point.X <= maxX && point.Y >= minY && point.Y <= maxY;
    }

    // Tujuan: ambil titik outline shape setelah diterapkan transform global.
    // Input: instance shape aktif yang mau dihitung.
    // Output: balikin list titik Vector2 yang sudah diposisikan di dunia.
    private List<Vector2> BuildTransformedPoints(ShapeInstance instance) {
        return BuildTransformedPointsScaled(instance.Definition, instance.WorldPivot, instance.Rotation, _globalShapeScale);
    }

    // Tujuan: transform outline shape tertentu dengan pivot dan rotasi tertentu.
    // Input: shape definisi bentuk, worldPivot titik dunia, rotation rotasi radian.
    // Output: balikin list titik Vector2 yang sudah diputar dan dipindah.
    private List<Vector2> BuildTransformedPoints(PrebuiltShape shape, Vector2 worldPivot, float rotation) {
        return BuildTransformedPointsScaled(shape, worldPivot, rotation, _globalShapeScale);
    }

    // Tujuan: transform outline shape dengan tambahan parameter skala custom.
    // Input: shape definisi dasar, worldPivot posisi target, rotation rotasi radian, scale faktor skala.
    // Output: balikin list titik Vector2 hasil transform penuh.
    private List<Vector2> BuildTransformedPointsScaled(PrebuiltShape shape, Vector2 worldPivot, float rotation, float scale) {
        Matrix4x4 matrix = TransformasiFast.Identity();
        if (!Mathf.IsEqualApprox(scale, 1f)) _transformasi.Scaling(ref matrix, scale, scale, shape.LocalPivot);
        _transformasi.RotationClockwise(ref matrix, rotation, shape.LocalPivot);
        Vector2 translation = worldPivot - shape.LocalPivot;
        _transformasi.Translation(ref matrix, translation.X, translation.Y);
        return _transformasi.GetTransformPoint(matrix, shape.BasePoints);
    }

    // Tujuan: transform vertex isi shape supaya bisa dipakai Godot.DrawPolygon.
    // Input: shape definisi dasar, worldPivot posisi, rotation rotasi radian, scale faktor skala.
    // Output: array Vector2 hasil transform atau kosong kalau shape gak punya fill.
    private Vector2[] BuildTransformedFillVertices(PrebuiltShape shape, Vector2 worldPivot, float rotation, float scale) {
        if (shape.FillVertices == null || shape.FillVertices.Length == 0) return Array.Empty<Vector2>();

        Matrix4x4 matrix = TransformasiFast.Identity();
        if (!Mathf.IsEqualApprox(scale, 1f)) _transformasi.Scaling(ref matrix, scale, scale, shape.LocalPivot);
        _transformasi.RotationClockwise(ref matrix, rotation, shape.LocalPivot);
        Vector2 translation = worldPivot - shape.LocalPivot;
        _transformasi.Translation(ref matrix, translation.X, translation.Y);

        var results = new Vector2[shape.FillVertices.Length];
        for (int i = 0; i < shape.FillVertices.Length; i++) {
            Vector2 pt = shape.FillVertices[i];
            System.Numerics.Vector3 temp = new System.Numerics.Vector3(pt.X, pt.Y, 1f);
            var transformed = new System.Numerics.Vector3(
                matrix.M11 * temp.X + matrix.M12 * temp.Y + matrix.M13 * temp.Z + matrix.M14,
                matrix.M21 * temp.X + matrix.M22 * temp.Y + matrix.M23 * temp.Z + matrix.M24,
                matrix.M31 * temp.X + matrix.M32 * temp.Y + matrix.M33 * temp.Z + matrix.M34
            );
            results[i] = new Vector2(transformed.X, transformed.Y);
        }

        return results;
    }

    // Tujuan: ambil definisi shape dari cache atau bangun baru kalau belum ada.
    // Input: type tipe shape yang diminta.
    // Output: balikin PrebuiltShape berisi outline, fill, pivot, dan warna.
    private PrebuiltShape GetShapeDefinition(ShapeType type) {
        if (_shapeCache.TryGetValue(type, out var cached)) return cached;

        _shapeLibrary.UnitSize = _unitSize;
        var data = _shapeLibrary.CreateShape(type, Vector2.Zero, filled: true);

        cached = new PrebuiltShape {
            BasePoints = data.OutlinePoints,
            FillVertices = data.FillVertices,
            LocalPivot = data.Pivot,
            Color = data.Color
        };

        _shapeCache[type] = cached;
        return cached;
    }

    // Tujuan: dapatkan outline template yang sudah diskalakan untuk tampilan overlay.
    // Input: type bentuk, pivot posisi dunia, rotation rotasi radian, color output warna bentuk.
    // Output: balikin list titik Vector2 hasil transform template.
    protected List<Vector2> GetTemplateTransformedPoints(ShapeType type, Vector2 pivot, float rotation, out Color color) {
        var definition = GetShapeDefinition(type);
        if (definition == null) {
            color = Colors.White;
            return new List<Vector2>();
        }

        color = definition.Color;
        return BuildTransformedPointsScaled(definition, pivot, rotation, _globalShapeScale);
    }

    // Tujuan: ambil vertex isi template yang sudah siap digambar.
    // Input: type bentuk, pivot posisi dunia, rotation rotasi radian, color output warna bentuk.
    // Output: array Vector2 hasil transform atau kosong kalau definisi tidak tersedia.
    protected Vector2[] GetTemplateFillVertices(ShapeType type, Vector2 pivot, float rotation, out Color color) {
        var definition = GetShapeDefinition(type);
        if (definition == null) {
            color = Colors.White;
            return Array.Empty<Vector2>();
        }

        color = definition.Color;
        return BuildTransformedFillVertices(definition, pivot, rotation, _globalShapeScale);
    }

    // Tujuan: ambil label tampilan shape berdasarkan mapping yang ada.
    // Input: type tipe shape yang dicari.
    // Output: string nama shape untuk ditampilkan ke user.
    private string GetShapeDisplayName(ShapeType type) {
        if (_shapeLabels.TryGetValue(type, out var label)) return label;
        return type.ToString();
    }

    // Tujuan: get count untuk active shapes (bisa digunakan oleh subclass)
    protected int GetActiveShapeCount() {
        return _activeShapes.Count;
    }

    // Tujuan: get list entries untuk export (bisa digunakan oleh subclass)
    protected List<TemplateLoader.TemplateEntry> GetActiveShapeEntries() {
        var entries = new List<TemplateLoader.TemplateEntry>();
        foreach (var instance in _activeShapes) {
            entries.Add(new TemplateLoader.TemplateEntry {
                ShapeType = instance.ShapeType,
                Pivot = instance.WorldPivot,
                AngleRadians = instance.Rotation
            });
        }
        return entries;
    }

    // Tujuan: export layout shape ke file JSON untuk template stage
    public void ExportLayout() {
        if (_activeShapes.Count == 0) {
            GD.Print("[ShapePlayground] Tidak ada shape yang ditempatkan.");
            return;
        }

        // Print ke console untuk preview
        GD.Print("---- Shape Layout Snapshot ----");
        GD.Print($"Shapes: {_activeShapes.Count}, Scale: {ExportScale}");

        // Siapkan data untuk save ke JSON
        var entries = GetActiveShapeEntries();

        // Generate filename dengan timestamp
        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string filename = $"Template_{timestamp}.json";
        string folderPath = ProjectSettings.GlobalizePath("res://StagesReceipt/");
        string fullPath = System.IO.Path.Combine(folderPath, filename);

        // Pastikan folder exists
        if (!System.IO.Directory.Exists(folderPath)) {
            System.IO.Directory.CreateDirectory(folderPath);
        }

        // Save ke JSON
        bool success = TemplateLoader.SaveToJson(
            fullPath,
            $"Custom Template {timestamp}",
            ExportScale,
            entries
        );

        if (success) {
            GD.Print($"[ShapePlayground] ✓ Template disimpan: {filename}");
            GD.Print($"[ShapePlayground] Location: {fullPath}");
        } else {
            GD.PrintErr("[ShapePlayground] ✗ Gagal menyimpan template");
        }
    }
}
