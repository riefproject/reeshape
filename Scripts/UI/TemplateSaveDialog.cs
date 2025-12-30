using Godot;
using System;

public partial class TemplateSaveDialog : Control
{
	[Signal]
	public delegate void SaveConfirmedEventHandler(string filename);

	[Signal]
	public delegate void DialogClosedEventHandler();

	private LineEdit _filenameInput;
	private Label _errorLabel;
	private StyleBoxFlat _panelStyle;
	private StyleBoxEmpty _btnFocusStyle;
	private StyleBoxFlat _btnHoverStyle;
	private StyleBoxFlat _btnPressedStyle;
	private StyleBoxFlat _btnNormalStyle;
	private Tween _activeTween;

	public override void _Ready()
	{
		CreateStyles();
		BuildUI();
		Visible = false;
		MouseFilter = MouseFilterEnum.Ignore;
	}

	private void CreateStyles()
	{
		// Panel style - dark blue with border and shadow
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

	private void BuildUI()
	{
		// Dimmer background
		var dimmer = new ColorRect
		{
			Color = new Color(0, 0, 0, 0.5f),
			MouseFilter = MouseFilterEnum.Stop
		};
		dimmer.SetAnchorsPreset(LayoutPreset.FullRect);
		AddChild(dimmer);

		// Center panel
		var panel = new PanelContainer();
		panel.SetAnchorsPreset(LayoutPreset.Center);
		panel.AnchorLeft = 0.5f;
		panel.AnchorRight = 0.5f;
		panel.AnchorTop = 0.5f;
		panel.AnchorBottom = 0.5f;
		panel.OffsetLeft = -250;
		panel.OffsetRight = 250;
		panel.OffsetTop = -140;
		panel.OffsetBottom = 140;
		panel.GrowHorizontal = Control.GrowDirection.Both;
		panel.GrowVertical = Control.GrowDirection.Both;
		panel.AddThemeStyleboxOverride("panel", _panelStyle);
		AddChild(panel);

		// Margin
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
			Text = "SIMPAN TEMPLATE",
			HorizontalAlignment = HorizontalAlignment.Center
		};
		title.AddThemeFontSizeOverride("font_size", 24);
		title.AddThemeColorOverride("font_color", new Color(0.9f, 0.95f, 1f));
		vbox.AddChild(title);

		// Description
		var desc = new Label
		{
			Text = "Masukkan nama untuk file template",
			HorizontalAlignment = HorizontalAlignment.Center
		};
		desc.AddThemeFontSizeOverride("font_size", 14);
		desc.AddThemeColorOverride("font_color", new Color(0.7f, 0.8f, 0.9f));
		vbox.AddChild(desc);

		// Input field
		_filenameInput = new LineEdit
		{
			PlaceholderText = "Nama template (contoh: MyTemplate)",
			CustomMinimumSize = new Vector2(0, 42)
		};
		_filenameInput.AddThemeFontSizeOverride("font_size", 16);
		_filenameInput.TextSubmitted += (text) => OnSavePressed();
		vbox.AddChild(_filenameInput);

		// Error label
		_errorLabel = new Label
		{
			Text = "",
			HorizontalAlignment = HorizontalAlignment.Center,
			Visible = false
		};
		_errorLabel.AddThemeFontSizeOverride("font_size", 12);
		_errorLabel.AddThemeColorOverride("font_color", new Color(1f, 0.3f, 0.3f));
		vbox.AddChild(_errorLabel);

		// Buttons
		var btnContainer = new HBoxContainer
		{
			Alignment = BoxContainer.AlignmentMode.Center
		};
		btnContainer.AddThemeConstantOverride("separation", 16);
		vbox.AddChild(btnContainer);

		// Save button
		var saveBtn = new Button
		{
			Text = "Simpan",
			CustomMinimumSize = new Vector2(120, 42)
		};
		saveBtn.AddThemeStyleboxOverride("focus", _btnFocusStyle);
		saveBtn.AddThemeStyleboxOverride("hover", _btnHoverStyle);
		saveBtn.AddThemeStyleboxOverride("pressed", _btnPressedStyle);
		saveBtn.AddThemeStyleboxOverride("normal", _btnNormalStyle);
		saveBtn.AddThemeFontSizeOverride("font_size", 16);
		saveBtn.Pressed += OnSavePressed;
		btnContainer.AddChild(saveBtn);

		// Cancel button
		var cancelBtn = new Button
		{
			Text = "Batal",
			CustomMinimumSize = new Vector2(120, 42)
		};
		cancelBtn.AddThemeStyleboxOverride("focus", _btnFocusStyle);
		cancelBtn.AddThemeStyleboxOverride("hover", _btnHoverStyle);
		cancelBtn.AddThemeStyleboxOverride("pressed", _btnPressedStyle);
		cancelBtn.AddThemeStyleboxOverride("normal", _btnNormalStyle);
		cancelBtn.AddThemeFontSizeOverride("font_size", 16);
		cancelBtn.Pressed += Hide;
		btnContainer.AddChild(cancelBtn);
	}

	public void ShowDialog()
	{
		// Generate default filename
		string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
		_filenameInput.Text = $"Template_{timestamp}";
		_filenameInput.SelectAll();
		_errorLabel.Visible = false;

		Visible = true;
		MouseFilter = MouseFilterEnum.Stop;

		// Focus input
		_filenameInput.CallDeferred("grab_focus");

		// Fade-in animation
		Modulate = new Color(1f, 1f, 1f, 0f);
		_activeTween?.Kill();
		_activeTween = CreateTween();
		_activeTween.TweenProperty(this, "modulate:a", 1f, 0.25f)
			.SetTrans(Tween.TransitionType.Cubic)
			.SetEase(Tween.EaseType.Out);
	}

	private void OnSavePressed()
	{
		string filename = _filenameInput.Text.Trim();

		if (string.IsNullOrEmpty(filename))
		{
			ShowError("Nama file tidak boleh kosong!");
			return;
		}

		// Validate filename
		if (filename.IndexOfAny(System.IO.Path.GetInvalidFileNameChars()) >= 0)
		{
			ShowError("Nama file mengandung karakter tidak valid!");
			return;
		}

		EmitSignal(SignalName.SaveConfirmed, filename);
		Hide();
	}

	private void ShowError(string message)
	{
		_errorLabel.Text = message;
		_errorLabel.Visible = true;
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
