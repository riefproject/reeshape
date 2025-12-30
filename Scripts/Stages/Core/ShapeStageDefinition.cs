using Godot;
using System.Collections.Generic;
using ShapeType = PatternShapeLibrary.ShapeType;

public sealed class ShapeStageDefinition {
    public sealed class ShapeQuota {
        public ShapeType Type { get; }
        public int MaxCopies { get; }
        public int InitialMax { get; }
        public bool IsUnlimited { get; }
        public string DisplayName { get; }

        // Tujuan: inisialisasi kuota shape dengan batas jumlah yang diperbolehkan.
        // Input: type menentukan bentuk, maxCopies jumlah maksimal (negatif berarti tak terbatas), displayName label opsional.
        public ShapeQuota(ShapeType type, int maxCopies, string displayName = null) {
            Type = type;
            if (maxCopies < 0) {
                IsUnlimited = true;
                MaxCopies = int.MaxValue;
                InitialMax = -1;
            } else {
                IsUnlimited = false;
                MaxCopies = maxCopies;
                InitialMax = maxCopies;
            }

            DisplayName = displayName ?? type.ToString();
        }
    }

    private readonly List<ShapeQuota> _quotas = new();

    public IReadOnlyList<ShapeQuota> Quotas => _quotas;

    public int UnitSize { get; set; } = 60;

    public float RotationStepDegrees { get; set; } = 15f;

    public float PaletteSpacing { get; set; } = 180f;

    public Vector2 PaletteStart { get; set; } = new Vector2(180f, 600f);

    public Vector2 PaletteTileSize { get; set; } = new Vector2(140f, 140f);

    public float PalettePreviewScale { get; set; } = 0.8f;

    // Tujuan: menambah kuota shape ke definisi stage secara fluent.
    // Input: type tipe shape, maxCopies batas jumlah, displayName label yang tampil di UI.
    // Output: balikin instance ShapeStageDefinition yang sama untuk chaining.
    public ShapeStageDefinition AddQuota(ShapeType type, int maxCopies, string displayName = null) {
        _quotas.Add(new ShapeQuota(type, maxCopies, displayName));
        return this;
    }
}

public static class ShapeStageLibrary {
    // Tujuan: bikin definisi stage standar dengan kuota terbatas untuk mode biasa.
    // Output: balikin ShapeStageDefinition siap dipakai stage default.
    public static ShapeStageDefinition DefaultStage() {
        return new ShapeStageDefinition {
            UnitSize = 70,
            RotationStepDegrees = 15f,
            PaletteSpacing = 170f,
            PaletteStart = new Vector2(200f, 620f),
            PaletteTileSize = new Vector2(130f, 130f),
            PalettePreviewScale = 0.6f
        }
        .AddQuota(ShapeType.Persegi, 4, "Persegi")
        .AddQuota(ShapeType.Segitiga, 6, "Segitiga")
        .AddQuota(ShapeType.Trapesium, 3, "Trapesium")
        .AddQuota(ShapeType.JajarGenjang, 4, "Jajar Genjang")
        .AddQuota(ShapeType.JajarGenjang2, 4, "Belah Ketupat")
        .AddQuota(ShapeType.Hexagon, 2, "Hexagon");
    }

    // Tujuan: bikin definisi stage dengan kuota tak terbatas untuk mode catat rekor.
    // Output: balikin ShapeStageDefinition tanpa batas jumlah shape.
    public static ShapeStageDefinition RecordStage() {
        return new ShapeStageDefinition {
            UnitSize = 70,
            RotationStepDegrees = 15f,
            PaletteSpacing = 160f,
            PaletteStart = new Vector2(200f, 620f),
            PaletteTileSize = new Vector2(130f, 130f),
            PalettePreviewScale = 0.6f
        }
        .AddQuota(ShapeType.Persegi, -1, "Persegi")
        .AddQuota(ShapeType.Segitiga, -1, "Segitiga")
        .AddQuota(ShapeType.Trapesium, -1, "Trapesium")
        .AddQuota(ShapeType.JajarGenjang, -1, "Jajar Genjang")
        .AddQuota(ShapeType.JajarGenjang2, -1, "Belah Ketupat")
        .AddQuota(ShapeType.Hexagon, -1, "Hexagon");
    }
}
