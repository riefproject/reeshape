using Godot;
using System.Collections.Generic;
using ShapeType = PatternShapeLibrary.ShapeType;

public partial class TemplateShapeStage : ShapePlayground {
    protected sealed class TemplateSlot {
        public TemplateLoader.TemplateEntry Entry { get; init; }
        public bool IsOccupied { get; set; }
        public bool IsPreview { get; set; }
        public ShapeInstance Occupant { get; set; }
    }

    [Export]
    public string ReceiptPath { get; set; } = string.Empty;

    [Export]
    public float SnapDistancePixels { get; set; } = 18f;

    [Export]
    public float AngleToleranceDegrees { get; set; } = 10f;

    private float _templateScale = 1f;

    [Export(PropertyHint.Range, "0.1,5,0.05")]
    public float TemplateScale { get => _templateScale; set => _templateScale = Mathf.Max(0.01f, value); }

    [Export]
    public bool TemplateAutoCenter { get; set; } = true;

    [Export]
    public Vector2 TemplateOffset { get; set; } = Vector2.Zero;

    [Export]
    public float TemplateUnitSize { get; set; } = 70f;

    [Export]
    public float TemplateRotationStep { get; set; } = 15f;

    [Export]
    public float TemplatePaletteSpacing { get; set; } = 160f;

    [Export]
    public Vector2 TemplatePaletteStart { get; set; } = new Vector2(200f, 620f);

    [Export]
    public Vector2 TemplatePaletteTileSize { get; set; } = new Vector2(130f, 130f);

    [Export]
    public float TemplatePalettePreviewScale { get; set; } = 0.6f;

    [Export]
    public NodePath TimerLabelPath { get; set; } = new NodePath("TimerLabel");

    [Export]
    public NodePath BestTimeLabelPath { get; set; } = new NodePath("BestTimeLabel");

    [Export]
    public NodePath StageTitleLabelPath { get; set; } = new NodePath("StageLabel");

    [Export]
    public NodePath CompletionPopupPath { get; set; } = new NodePath("StageCompletionPopup");

    [Export]
    public NodePath BackButtonContainerPath { get; set; } = new NodePath("HBoxContainer2");

    [Export]
    public NodePath MainTitleLabelPath { get; set; } = new NodePath("Title");

    [Export]
    public NodePath GuideButtonPath { get; set; } = new NodePath("GuideButton");

    [Export]
    public string StageId { get; set; } = string.Empty;

    [Export]
    public string StageDisplayName { get; set; } = string.Empty;

    [Export]
    public NodePath StatusLabelPath { get; set; } = new NodePath("StatusLabel");

    [Export]
    public Color SlotOutlineColor { get; set; } = new Color(0.65f, 0.65f, 0.65f, 0.75f);

    [Export]
    public Color SlotFillColor { get; set; } = new Color(0.65f, 0.65f, 0.65f, 0.22f);

    [Export]
    public Color PreviewOutlineColor { get; set; } = new Color(0.9f, 0.9f, 0.9f, 0.85f);

    [Export]
    public Color PreviewFillColor { get; set; } = new Color(0.9f, 0.9f, 0.9f, 0.3f);

    private TemplateLoader.ShapeTemplate _template;
    private readonly List<TemplateSlot> _slots = new();
    private readonly Dictionary<ShapeInstance, TemplateSlot> _shapeToSlot = new();
    private readonly Dictionary<ShapeInstance, TemplateSlot> _previewSlots = new();

    private Label _statusLabel;
    private Label _timerLabel;
    private Label _bestTimeLabel;
    private Label _stageTitleLabel;
    private StageCompletionPopup _completionPopup;
    private Control _backButtonContainer;
    private Label _mainTitleLabel;
    private bool _templateCompleted;
    private float _snapDistanceBase;
    private float _templateScaleApplied = 1f;
    private double _elapsedSeconds;
    private bool _timerRunning;
    private bool _stageSessionInitialized;
    private StageSaveEntry _lastRecord;
    private string _resolvedStageId = string.Empty;
    private string _resolvedStageName = string.Empty;
    private StageGuideOverlay _guideOverlay;
    private Button _guideButton;

    protected TemplateLoader.ShapeTemplate Template => _template;
    protected float TemplateScaleApplied => _templateScaleApplied;
}
