using Godot;
using System.Collections.Generic;
using ShapeType = PatternShapeLibrary.ShapeType;

public partial class ShapePlayground : Node2D {
    protected sealed class PrebuiltShape {
        public List<Vector2> BasePoints { get; init; } = new();
        public Vector2[] FillVertices { get; init; } = System.Array.Empty<Vector2>();
        public Vector2 LocalPivot { get; init; }
        public Color Color { get; init; }
    }

    private sealed class PaletteEntry {
        public ShapeStageDefinition.ShapeQuota Quota { get; init; }
        public int Remaining { get; set; }
        public bool IsUnlimited { get; init; }
        public Vector2 PreviewPosition { get; init; }
        public PrebuiltShape Shape { get; init; }
    }

    protected sealed class ShapeInstance {
        public PrebuiltShape Definition { get; init; }
        public Vector2 WorldPivot { get; set; }
        public float Rotation { get; set; }
        public int PaletteIndex { get; init; }
        public ShapeType ShapeType { get; init; }
        public bool IsLocked { get; set; }
    }
}
