using Godot;
using System.Collections.Generic;

public partial class Projects : Control {
	// Tujuan: atur background supaya bergeser ketika halaman projects dibuka.
	public override void _Ready() {
		var background = GetNode<Node>("/root/BG");
		background.Call("slide_to_anchor_y", 120.0);
		StageSaveService.EnsureLoaded();
		ApplyStageCompletionHighlights();
	}

	// Tujuan: ubah tampilan tombol stage jika pemain sudah menyelesaikan stage tersebut secara konsisten.
	private void ApplyStageCompletionHighlights() {
		var uniformPalette = new StageCompletionPalette(
			normalBg: new Color(0.086f, 0.353f, 0.325f, 0.92f),
			normalBorder: new Color(0.239f, 0.651f, 0.596f, 0.95f),
			hoverBg: new Color(0.118f, 0.462f, 0.427f, 1f),
			hoverBorder: new Color(0.325f, 0.788f, 0.722f, 1f),
			pressedBg: new Color(0.063f, 0.27f, 0.251f, 1f),
			pressedBorder: new Color(0.204f, 0.529f, 0.486f, 0.94f),
			fontColor: new Color(0.933f, 0.988f, 0.965f, 1f)
		);

		var configs = new[] {
			new StageButtonConfig("ufo", "PanelContainer/GridContainer/BtnProjectUFO", "UFO\nSTAGE", uniformPalette, this),
			new StageButtonConfig("rocket", "PanelContainer/GridContainer/BtnProjectRocket", "ROCKET\nSTAGE", uniformPalette, this),
			new StageButtonConfig("astronaut", "PanelContainer/GridContainer/BtnProjectAstronout", "ASTRONOUT\nSTAGE", uniformPalette, this)
		};

		foreach (var config in configs) config.Apply(StageSaveService.GetRecord(config.StageId));
	}

	// Tujuan: helper umum buat ganti scene ke path tertentu.
	// Input: scenePath path resource yang mau dibuka.
	private void ChangeScene(string scenePath) {
		if (string.IsNullOrEmpty(scenePath)) return;
		GetTree().ChangeSceneToFile(scenePath);
	}

	// Tujuan: balik ke halaman welcome dari halaman projects.
	private void _on_BtnBack_pressed() {
		ChangeScene("res://Scenes/Pages/Welcome.tscn");
	}

	// Tujuan: buka halaman 404 ketika konten belum tersedia.
	private void PageNotFound() {
		ChangeScene("res://Scenes/Pages/404.tscn");
	}

	// Tujuan: buka karya nomor 1.
	private void PageKarya1() {
		ChangeScene("res://Scenes/Karya/Karya1.tscn");
	}

	// Tujuan: buka karya nomor 2.
	private void PageKarya2() {
		ChangeScene("res://Scenes/Karya/Karya2.tscn");
	}

	// Tujuan: buka karya nomor 3.
	private void PageKarya3() {
		ChangeScene("res://Scenes/Karya/Karya3.tscn");
	}

	// Tujuan: buka karya nomor 4.
	private void PageKarya4() {
		ChangeScene("res://Scenes/Karya/Karya4.tscn");
	}

	// Tujuan: buka karya nomor 5.
	private void PageKarya5() {
		ChangeScene("res://Scenes/Karya/Karya5.tscn");
	}

	// Tujuan: buka halaman builder template custom.
	private void PageTemplateBuilder() {
		ChangeScene("res://Scenes/Karya/TemplateBuilder.tscn");
	}

	// Tujuan: buka stage UFO.
	private void PageUFO() {
		DynamicPatternData.TemplatePath = "res://StagesReceipt/UFO.json";
		DynamicPatternData.PatternName = "UFO Stage";
		DynamicPatternData.Difficulty = "EASY";
		DynamicPatternData.ReturnScene = "res://Scenes/Pages/Projects.tscn";
		ChangeScene("res://Scenes/Karya/DynamicPatternStage.tscn");
	}

	// Tujuan: buka stage Astronout.
	private void PageAstronout() {
		DynamicPatternData.TemplatePath = "res://StagesReceipt/Astronout.json";
		DynamicPatternData.PatternName = "Astronout Stage";
		DynamicPatternData.Difficulty = "HARD";
		DynamicPatternData.ReturnScene = "res://Scenes/Pages/Projects.tscn";
		ChangeScene("res://Scenes/Karya/DynamicPatternStage.tscn");
	}

	// Tujuan: buka stage Rocket.
	private void PageRocket() {
		DynamicPatternData.TemplatePath = "res://StagesReceipt/Rocket.json";
		DynamicPatternData.PatternName = "Rocket Stage";
		DynamicPatternData.Difficulty = "MEDIUM";
		DynamicPatternData.ReturnScene = "res://Scenes/Pages/Projects.tscn";
		ChangeScene("res://Scenes/Karya/DynamicPatternStage.tscn");
	}

	// Tujuan: buka halaman My Patterns untuk custom user templates.
	private void PageMyPatterns() {
		ChangeScene("res://Scenes/Pages/MyPatterns.tscn");
	}

	private static void ApplyCompletedTheme(Button button, StageCompletionPalette palette) {
		if (button == null || palette == null) return;

		button.AddThemeStyleboxOverride("normal", BuildStyle(palette.NormalBg, palette.NormalBorder, new Vector2(3f, 3f)));
		button.AddThemeStyleboxOverride("hover", BuildStyle(palette.HoverBg, palette.HoverBorder, new Vector2(3f, 3f)));
		button.AddThemeStyleboxOverride("pressed", BuildStyle(palette.PressedBg, palette.PressedBorder, new Vector2(1.5f, 1.5f)));
		button.AddThemeStyleboxOverride("disabled", BuildStyle(palette.PressedBg, palette.PressedBorder, new Vector2(1.5f, 1.5f)));
		button.AddThemeColorOverride("font_color", palette.FontColor);
	}

	private static StyleBoxFlat BuildStyle(Color background, Color border, Vector2 shadowOffset) {
		var style = new StyleBoxFlat();
		style.BgColor = background;
		style.BorderColor = border;
		style.BorderWidthTop = 3;
		style.BorderWidthBottom = 0;
		style.BorderWidthLeft = 0;
		style.BorderWidthRight = 0;
		style.CornerRadiusTopLeft = 20;
		style.CornerRadiusTopRight = 20;
		style.CornerRadiusBottomLeft = 20;
		style.CornerRadiusBottomRight = 20;
		style.ShadowSize = 6;
		style.ShadowOffset = shadowOffset;
		style.ShadowColor = new Color(0, 0, 0, 0.35f);
		return style;
	}

	private sealed class StageButtonConfig {
		private readonly Control _owner;
		private readonly StyleBox _defaultNormal;
		private readonly StyleBox _defaultHover;
		private readonly StyleBox _defaultPressed;
		private readonly StyleBox _defaultDisabled;
		private readonly Color _defaultFontColor;
		private readonly bool _hadNormalOverride;
		private readonly bool _hadHoverOverride;
		private readonly bool _hadPressedOverride;
		private readonly bool _hadDisabledOverride;
		private readonly bool _hadFontColorOverride;

		public StageButtonConfig(string stageId, string buttonPath, string defaultText, StageCompletionPalette palette, Control owner) {
			StageId = stageId;
			DefaultText = defaultText;
			Palette = palette;
			_owner = owner;
			Button = owner?.GetNodeOrNull<Button>(buttonPath);

			if (Button != null) {
				_hadNormalOverride = Button.HasThemeStyleboxOverride("normal");
				_hadHoverOverride = Button.HasThemeStyleboxOverride("hover");
				_hadPressedOverride = Button.HasThemeStyleboxOverride("pressed");
				_hadDisabledOverride = Button.HasThemeStyleboxOverride("disabled");
				_hadFontColorOverride = Button.HasThemeColorOverride("font_color");

				_defaultNormal = _hadNormalOverride ? Button.GetThemeStylebox("normal") : null;
				_defaultHover = _hadHoverOverride ? Button.GetThemeStylebox("hover") : null;
				_defaultPressed = _hadPressedOverride ? Button.GetThemeStylebox("pressed") : null;
				_defaultDisabled = _hadDisabledOverride ? Button.GetThemeStylebox("disabled") : null;
				_defaultFontColor = _hadFontColorOverride ? Button.GetThemeColor("font_color") : default;
			}
		}

		public string StageId { get; }
		public Button Button { get; }
		public string DefaultText { get; }
		public StageCompletionPalette Palette { get; }

		public void Apply(StageSaveEntry record) {
			if (Button == null) return;

			if (record != null && record.Completed) {
				ApplyCompletedTheme(Button, Palette);
				Button.Text = BuildCompletedLabel();
				string best = TimeFormatUtils.FormatElapsedTime(record.BestTimeSeconds, "--:--");
				Button.TooltipText = $"Stage ini pernah kamu selesaikan. Rekor waktu: {best}";
			} else {
				RestoreDefaults();
				Button.Text = DefaultText;
				Button.TooltipText = "Belum diselesaikan";
			}
		}

		private void RestoreDefaults() {
			if (Button == null) return;

			if (_hadNormalOverride && _defaultNormal != null) Button.AddThemeStyleboxOverride("normal", _defaultNormal);
			else Button.RemoveThemeStyleboxOverride("normal");

			if (_hadHoverOverride && _defaultHover != null) Button.AddThemeStyleboxOverride("hover", _defaultHover);
			else Button.RemoveThemeStyleboxOverride("hover");

			if (_hadPressedOverride && _defaultPressed != null) Button.AddThemeStyleboxOverride("pressed", _defaultPressed);
			else Button.RemoveThemeStyleboxOverride("pressed");

			if (_hadDisabledOverride && _defaultDisabled != null) Button.AddThemeStyleboxOverride("disabled", _defaultDisabled);
			else Button.RemoveThemeStyleboxOverride("disabled");

			if (_hadFontColorOverride) Button.AddThemeColorOverride("font_color", _defaultFontColor);
			else Button.RemoveThemeColorOverride("font_color");
		}

		private string BuildCompletedLabel() {
			string primary = DefaultText;
			if (string.IsNullOrWhiteSpace(primary)) return "STAGE SELESAI";
			int newlineIndex = primary.IndexOf('\n');
			if (newlineIndex >= 0) primary = primary[..newlineIndex];
			primary = primary.Trim();
			if (string.IsNullOrEmpty(primary)) primary = "STAGE";
			return string.Format("{0}\nSTAGE SELESAI", primary);
		}
	}

	private sealed class StageCompletionPalette {
		public StageCompletionPalette(Color normalBg, Color normalBorder, Color hoverBg, Color hoverBorder, Color pressedBg, Color pressedBorder, Color fontColor) {
			NormalBg = normalBg;
			NormalBorder = normalBorder;
			HoverBg = hoverBg;
			HoverBorder = hoverBorder;
			PressedBg = pressedBg;
			PressedBorder = pressedBorder;
			FontColor = fontColor;
		}

		public Color NormalBg { get; }
		public Color NormalBorder { get; }
		public Color HoverBg { get; }
		public Color HoverBorder { get; }
		public Color PressedBg { get; }
		public Color PressedBorder { get; }
		public Color FontColor { get; }
	}
}
