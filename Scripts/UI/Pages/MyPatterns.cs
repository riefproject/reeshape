using Godot;
using System.Collections.Generic;
using System.Linq;

public partial class MyPatterns : Control
{
	[Export]
	public NodePath BackButtonPath { get; set; } = new NodePath("BackButton");

	[Export]
	public NodePath TitleLabelPath { get; set; } = new NodePath("TitleLabel");

	[Export]
	public NodePath PatternsContainerPath { get; set; } = new NodePath("ScrollContainer/PatternsGrid");

	private Button _backButton;
	private Label _titleLabel;
	private GridContainer _patternsGrid;

	// Default patterns yang tidak ditampilkan di My Patterns
	private static readonly string[] DefaultPatterns = { "UFO", "Rocket", "Astronout" };

	public override void _Ready()
	{
		InitializeNodes();
		LoadCustomPatterns();
	}

	private void InitializeNodes()
	{
		_backButton = GetNodeOrNull<Button>(BackButtonPath);
		_titleLabel = GetNodeOrNull<Label>(TitleLabelPath);
		_patternsGrid = GetNodeOrNull<GridContainer>(PatternsContainerPath);

		if (_backButton != null)
		{
			_backButton.Pressed += OnBackPressed;
		}
	}

	private void LoadCustomPatterns()
	{
		if (_patternsGrid == null)
		{
			GD.PrintErr("[MyPatterns] PatternsGrid tidak ditemukan");
			return;
		}

		// Clear existing children
		foreach (var child in _patternsGrid.GetChildren())
		{
			child.QueueFree();
		}

		// Load semua JSON files kecuali default patterns
		string folderPath = ProjectSettings.GlobalizePath("res://StagesReceipt/");
		var allFiles = TemplateLoader.GetAllTemplateFiles(folderPath);

		var customFiles = allFiles.Where(file =>
		{
			string filename = System.IO.Path.GetFileNameWithoutExtension(file);
			return !DefaultPatterns.Contains(filename);
		}).ToList();

		if (customFiles.Count == 0)
		{
			ShowEmptyMessage();
			return;
		}

		// Create button untuk setiap custom pattern
		foreach (var filePath in customFiles)
		{
			string filename = System.IO.Path.GetFileNameWithoutExtension(filePath);
			CreatePatternButton(filename, filePath);
		}
	}

	private void CreatePatternButton(string displayName, string filePath)
	{
		// Create margin container untuk padding
		var marginContainer = new MarginContainer();
		marginContainer.AddThemeConstantOverride("margin_left", 8);
		marginContainer.AddThemeConstantOverride("margin_right", 8);
		marginContainer.AddThemeConstantOverride("margin_top", 4);
		marginContainer.AddThemeConstantOverride("margin_bottom", 4);

		// Truncate nama jika terlalu panjang (max 20 karakter)
		string truncatedName = TruncateText(displayName, 20);

		var button = new Button
		{
			Text = truncatedName,
			CustomMinimumSize = new Vector2(280, 80),
			SizeFlagsHorizontal = SizeFlags.Fill,
			SizeFlagsVertical = SizeFlags.Fill
		};

		button.AddThemeFontSizeOverride("font_size", 18);

		// Apply consistent button styling
		ApplyButtonStyle(button);

		string capturedPath = filePath;
		string capturedName = displayName; // Gunakan nama asli untuk stage
		button.Pressed += () => OnPatternSelected(capturedPath, capturedName);

		marginContainer.AddChild(button);
		_patternsGrid.AddChild(marginContainer);
	}

	// Tujuan: truncate text jika melebihi max length
	// Input: text adalah string yang akan di-truncate, maxLength adalah panjang maksimal
	// Output: string yang sudah di-truncate dengan "..." jika perlu
	private string TruncateText(string text, int maxLength)
	{
		if (string.IsNullOrEmpty(text) || text.Length <= maxLength)
		{
			return text;
		}

		// Hitung berapa karakter yang diambil (maxLength - 3 untuk "...")
		int takeChars = maxLength - 3;
		return text.Substring(0, takeChars) + "...";
	}

	private void ApplyButtonStyle(Button button)
	{
		// Normal state
		var styleNormal = new StyleBoxFlat
		{
			BgColor = new Color(0.00392157f, 0.396078f, 0.67451f, 0.6f),
			CornerRadiusTopLeft = 20,
			CornerRadiusTopRight = 20,
			CornerRadiusBottomRight = 20,
			CornerRadiusBottomLeft = 20,
			BorderWidthTop = 3,
			BorderColor = new Color(0.00784314f, 0.466667f, 0.792157f, 1.0f),
			ShadowColor = new Color(0, 0, 0, 0.301961f),
			ShadowSize = 6,
			ShadowOffset = new Vector2(3, 3)
		};

		// Hover state
		var styleHover = new StyleBoxFlat
		{
			BgColor = new Color(0.0156863f, 0.537255f, 0.901961f, 1.0f),
			CornerRadiusTopLeft = 20,
			CornerRadiusTopRight = 20,
			CornerRadiusBottomRight = 20,
			CornerRadiusBottomLeft = 20,
			BorderWidthTop = 3,
			BorderColor = new Color(0.00784314f, 0.466667f, 0.792157f, 1.0f),
			ShadowColor = new Color(0, 0, 0, 0.301961f),
			ShadowSize = 6,
			ShadowOffset = new Vector2(3, 3)
		};

		// Pressed state
		var stylePressed = new StyleBoxFlat
		{
			BgColor = new Color(0.00392157f, 0.396078f, 0.67451f, 0.6f),
			CornerRadiusTopLeft = 20,
			CornerRadiusTopRight = 20,
			CornerRadiusBottomRight = 20,
			CornerRadiusBottomLeft = 20,
			BorderWidthTop = 3,
			BorderColor = new Color(0.00784314f, 0.466667f, 0.792157f, 1.0f),
			ShadowColor = new Color(0, 0, 0, 0.301961f),
			ShadowSize = 6,
			ShadowOffset = new Vector2(1, 1)
		};

		var styleFocus = new StyleBoxEmpty();

		button.AddThemeStyleboxOverride("normal", styleNormal);
		button.AddThemeStyleboxOverride("hover", styleHover);
		button.AddThemeStyleboxOverride("pressed", stylePressed);
		button.AddThemeStyleboxOverride("focus", styleFocus);
		button.AddThemeColorOverride("font_color", new Color(0.902951f, 0.924722f, 0.912399f, 1.0f));
	}

	private void ShowEmptyMessage()
	{
		var label = new Label
		{
			Text = "Belum ada custom pattern.\nBuat pattern di Template Builder!",
			HorizontalAlignment = HorizontalAlignment.Center,
			VerticalAlignment = VerticalAlignment.Center
		};
		label.AddThemeFontSizeOverride("font_size", 18);
		label.AddThemeColorOverride("font_color", new Color(0.7f, 0.7f, 0.7f));

		_patternsGrid.AddChild(label);
	}

	private void OnPatternSelected(string filePath, string patternName)
	{
		GD.Print($"[MyPatterns] Loading custom pattern: {patternName}");
		
		// Load ke dynamic stage dengan pattern ini
		const string dynamicStageScene = "res://Scenes/Karya/DynamicPatternStage.tscn";
		
		var error = GetTree().ChangeSceneToFile(dynamicStageScene);
		if (error != Error.Ok)
		{
			GD.PrintErr($"[MyPatterns] Gagal load scene: {error}");
		}
		else
		{
			// Pass data ke scene baru via DynamicPatternData
			DynamicPatternData.TemplatePath = filePath;
			DynamicPatternData.PatternName = patternName;
			DynamicPatternData.Difficulty = "Custom Pattern";
			DynamicPatternData.ReturnScene = "res://Scenes/Pages/MyPatterns.tscn";
		}
	}

	private void OnBackPressed()
	{
		const string playScene = "res://Scenes/Pages/Projects.tscn";
		GetTree().ChangeSceneToFile(playScene);
	}
}
