using Godot;
using System;
using System.Collections.Generic;

public partial class TemplatePickerDialog : Control
{
	[Signal]
	public delegate void TemplateSelectedEventHandler(string filePath);

	[Signal]
	public delegate void DialogClosedEventHandler();

	private List<string> _templateFiles = new();
	private StyleBoxFlat _panelStyle;
	private StyleBoxEmpty _btnFocusStyle;
	private StyleBoxFlat _btnHoverStyle;
	private StyleBoxFlat _btnPressedStyle;
	private StyleBoxFlat _btnNormalStyle;
	private Tween _activeTween;

	public override void _Ready()
	{
		CreateStyles();
		Visible = false;
		MouseFilter = MouseFilterEnum.Ignore;
	}

	private void CreateStyles()
	{
		// Panel style - dark blue with border and shadow (sama seperti settings panel)
		_panelStyle = new StyleBoxFlat
		{
			BgColor = new Color(0.0745098f, 0.129412f, 0.207843f, 0.95f),
			BorderWidthTop = 2,
			BorderColor = new Color(0.239216f, 0.588235f, 0.894118f, 0.85f),
			CornerRadiusTopLeft = 18,
			CornerRadiusTopRight = 18,
			CornerRadiusBottomRight = 18,
			CornerRadiusBottomLeft = 18,
			ShadowColor = new Color(0, 0, 0, 0.45f),
			ShadowSize = 18,
			ShadowOffset = new Vector2(0, 10)
		};

		// Button styles
		_btnFocusStyle = new StyleBoxEmpty();

		_btnHoverStyle = new StyleBoxFlat
		{
			BgColor = new Color(0.016705f, 0.538043f, 0.902577f, 1),
			BorderWidthTop = 1,
			BorderColor = new Color(0.00784314f, 0.466667f, 0.792157f, 1),
			CornerRadiusTopLeft = 5,
			CornerRadiusTopRight = 5,
			CornerRadiusBottomRight = 5,
			CornerRadiusBottomLeft = 5,
			ShadowColor = new Color(0, 0, 0, 0.3f),
			ShadowSize = 6,
			ShadowOffset = new Vector2(3, 3)
		};

		_btnPressedStyle = new StyleBoxFlat
		{
			BgColor = new Color(0.00392157f, 0.396078f, 0.67451f, 1),
			BorderWidthTop = 1,
			BorderColor = new Color(0.00392157f, 0.396078f, 0.67451f, 0.6f),
			CornerRadiusTopLeft = 5,
			CornerRadiusTopRight = 5,
			CornerRadiusBottomRight = 5,
			CornerRadiusBottomLeft = 5,
			ShadowColor = new Color(0, 0, 0, 0.3f),
			ShadowSize = 6,
			ShadowOffset = new Vector2(1, 1)
		};

		_btnNormalStyle = new StyleBoxFlat
		{
			BgColor = new Color(0.004f, 0.396f, 0.675f, 0.6f),
			BorderWidthTop = 1,
			BorderColor = new Color(0.00734803f, 0.468341f, 0.79103f, 1),
			CornerRadiusTopLeft = 5,
			CornerRadiusTopRight = 5,
			CornerRadiusBottomRight = 5,
			CornerRadiusBottomLeft = 5,
			ShadowColor = new Color(0, 0, 0, 0.3f),
			ShadowSize = 6,
			ShadowOffset = new Vector2(3, 3)
		};
	}

	public void ShowPicker(List<string> files)
	{
		_templateFiles = files ?? new List<string>();
		
		if (_templateFiles.Count == 0)
		{
			GD.Print("[TemplatePickerDialog] Tidak ada template untuk ditampilkan");
			return;
		}

		BuildUI();
		Visible = true;
		MouseFilter = MouseFilterEnum.Stop;
		
		// Fade-in animation
		Modulate = new Color(1f, 1f, 1f, 0f);
		_activeTween?.Kill();
		_activeTween = CreateTween();
		_activeTween.TweenProperty(this, "modulate:a", 1f, 0.25f)
			.SetTrans(Tween.TransitionType.Cubic)
			.SetEase(Tween.EaseType.Out);
	}

	private void BuildUI()
	{
		// Clear existing children
		foreach (var child in GetChildren())
		{
			child.QueueFree();
		}

		// Dimmer background
		var dimmer = new ColorRect
		{
			Color = new Color(0, 0, 0, 0.5f),
			MouseFilter = MouseFilterEnum.Stop
		};
		dimmer.SetAnchorsPreset(LayoutPreset.FullRect);
		AddChild(dimmer);

		// Center panel with custom style
		var panel = new PanelContainer();
		panel.SetAnchorsPreset(LayoutPreset.Center);
		panel.AnchorLeft = 0.5f;
		panel.AnchorRight = 0.5f;
		panel.AnchorTop = 0.5f;
		panel.AnchorBottom = 0.5f;
		panel.OffsetLeft = -290;  // Half of width (580/2)
		panel.OffsetRight = 290;
		panel.OffsetTop = -240;   // Half of height (480/2)
		panel.OffsetBottom = 240;
		panel.GrowHorizontal = Control.GrowDirection.Both;
		panel.GrowVertical = Control.GrowDirection.Both;
		panel.AddThemeStyleboxOverride("panel", _panelStyle);
		AddChild(panel);

		// Margin container
		var margin = new MarginContainer();
		margin.AddThemeConstantOverride("margin_left", 32);
		margin.AddThemeConstantOverride("margin_right", 32);
		margin.AddThemeConstantOverride("margin_top", 28);
		margin.AddThemeConstantOverride("margin_bottom", 28);
		panel.AddChild(margin);

		// Main VBox
		var vbox = new VBoxContainer();
		vbox.AddThemeConstantOverride("separation", 20);
		margin.AddChild(vbox);

		// Title
		var title = new Label
		{
			Text = "PILIH TEMPLATE",
			HorizontalAlignment = HorizontalAlignment.Center
		};
		title.AddThemeFontSizeOverride("font_size", 28);
		title.AddThemeColorOverride("font_color", new Color(0.9f, 0.95f, 1f));
		vbox.AddChild(title);

		// Description
		var desc = new Label
		{
			Text = "Pilih salah satu template untuk dimuat ke workspace",
			HorizontalAlignment = HorizontalAlignment.Center
		};
		desc.AddThemeFontSizeOverride("font_size", 14);
		desc.AddThemeColorOverride("font_color", new Color(0.7f, 0.8f, 0.9f));
		vbox.AddChild(desc);

		// Scroll container for file list
		var scroll = new ScrollContainer
		{
			CustomMinimumSize = new Vector2(0, 280),
			SizeFlagsVertical = SizeFlags.ExpandFill
		};
		vbox.AddChild(scroll);

		var fileList = new VBoxContainer();
		fileList.AddThemeConstantOverride("separation", 10);
		scroll.AddChild(fileList);

		// Populate file list dengan styled buttons
		foreach (var filePath in _templateFiles)
		{
			string filename = System.IO.Path.GetFileNameWithoutExtension(filePath);
			
			var btnMargin = new MarginContainer();
			btnMargin.AddThemeConstantOverride("margin_left", 8);
			btnMargin.AddThemeConstantOverride("margin_right", 8);
			fileList.AddChild(btnMargin);
			
			var btn = new Button
			{
				Text = filename,
				CustomMinimumSize = new Vector2(0, 45),
				SizeFlagsHorizontal = SizeFlags.ExpandFill
			};

			// Apply button styles
			btn.AddThemeStyleboxOverride("focus", _btnFocusStyle);
			btn.AddThemeStyleboxOverride("hover", _btnHoverStyle);
			btn.AddThemeStyleboxOverride("pressed", _btnPressedStyle);
			btn.AddThemeStyleboxOverride("normal", _btnNormalStyle);
			btn.AddThemeFontSizeOverride("font_size", 16);
			btn.AddThemeConstantOverride("h_separation", 16);  // Padding horizontal

			string capturedPath = filePath;
			btn.Pressed += () => OnTemplateButtonPressed(capturedPath);
			btnMargin.AddChild(btn);
		}

		// Buttons container
		var btnContainer = new HBoxContainer
		{
			Alignment = BoxContainer.AlignmentMode.Center
		};
		btnContainer.AddThemeConstantOverride("separation", 16);
		vbox.AddChild(btnContainer);

		// Close button
		var closeBtn = new Button
		{
			Text = "Batal",
			CustomMinimumSize = new Vector2(140, 42)
		};
		closeBtn.AddThemeStyleboxOverride("focus", _btnFocusStyle);
		closeBtn.AddThemeStyleboxOverride("hover", _btnHoverStyle);
		closeBtn.AddThemeStyleboxOverride("pressed", _btnPressedStyle);
		closeBtn.AddThemeStyleboxOverride("normal", _btnNormalStyle);
		closeBtn.AddThemeFontSizeOverride("font_size", 16);
		closeBtn.Pressed += Hide;
		btnContainer.AddChild(closeBtn);
	}

	private void OnTemplateButtonPressed(string filePath)
	{
		EmitSignal(SignalName.TemplateSelected, filePath);
		Hide();
	}

	public new void Hide()
	{
		if (!Visible) return;

		// Fade-out animation
		_activeTween?.Kill();
		_activeTween = CreateTween();
		_activeTween.TweenProperty(this, "modulate:a", 0f, 0.18f)
			.SetTrans(Tween.TransitionType.Cubic)
			.SetEase(Tween.EaseType.In);
		_activeTween.Finished += HideImmediate;
	}

	private void HideImmediate()
	{
		_activeTween?.Kill();
		Visible = false;
		MouseFilter = MouseFilterEnum.Ignore;
		Modulate = new Color(1f, 1f, 1f, 0f);
		EmitSignal(SignalName.DialogClosed);
	}
}
