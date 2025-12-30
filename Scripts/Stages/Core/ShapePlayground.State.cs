using Godot;
using System.Collections.Generic;

public partial class ShapePlayground : Node2D {
    [Export]
    public StagePreset SelectedStage { get; set; } = StagePreset.Default;

    [Export]
    public bool SnapToPixelGrid { get; set; } = false;

    private readonly PatternShapeLibrary _shapeLibrary = new();
    private readonly TransformasiFast _transformasi = new();

    private readonly Dictionary<PatternShapeLibrary.ShapeType, PrebuiltShape> _shapeCache = new();
    private readonly Dictionary<PatternShapeLibrary.ShapeType, string> _shapeLabels = new();
    private readonly List<PaletteEntry> _palette = new();
    private readonly List<ShapeInstance> _activeShapes = new();

    private Label _paletteTooltipLabel;
    private readonly Vector2 _paletteTooltipOffset = new(18f, 14f);
    private Vector2 _lastViewportSize = Vector2.Zero;

    private ShapeInstance _selectedShape;
    private ShapeInstance _draggedShape;
    private Vector2 _dragOffset;

    private Vector2 _viewOffset = Vector2.Zero;
    private float _viewScale = 1.0f;
    private bool _isPanning = false;

    private float _rotationStep = Mathf.DegToRad(15f);
    private float _exportScale = 1f;
    private float _globalShapeScale = 1f;
    private int _unitSize = 60;
    private float _paletteSpacing = 10f;
    private Vector2 _paletteStart = new(180f, 600f);
    private Vector2 _paletteTileSize = new(140f, 140f);
    private float _palettePreviewScale = 0.8f;

    private readonly Color _workspaceBg = new("15202b");
    private readonly Color _workspaceOutline = new("3e4a59");
    private readonly Color _paletteBg = new("202636");
    private readonly Color _paletteDisabledBg = new("141822");
    private readonly Color _paletteOutline = new("6c7a89");

    private Rect2 _libraryRect;

    [Export(PropertyHint.Range, "0.1,5,0.05")]
    public float ExportScale { get => _exportScale; set => _exportScale = Mathf.Max(0.01f, value); }

    protected float GlobalShapeScale => _globalShapeScale;
}
