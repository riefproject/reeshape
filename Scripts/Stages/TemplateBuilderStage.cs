using Godot;
using System;
using System.Globalization;
using System.IO;
using System.Collections.Generic;

public partial class TemplateBuilderStage : ShapePlayground {
	[Export]
	public NodePath ScaleControlPath { get; set; } = new NodePath("HBoxContainer2/ScalePanel/ScaleSpin");

	[Export]
	public NodePath ScalePanelPath { get; set; } = new NodePath("HBoxContainer2/ScalePanel");

	[Export]
	public NodePath BackButtonContainerPath { get; set; } = new NodePath("HBoxContainer2");

	[Export]
	public NodePath TitleLabelPath { get; set; } = new NodePath("Title");

	[Export]
	public NodePath ExportButtonPath { get; set; } = new NodePath("HBoxContainer2/ExportBtn");

	[Export]
	public NodePath LoadButtonPath { get; set; } = new NodePath("HBoxContainer2/LoadBtn");

	[Export]
	public string ReceiptPath { get; set; } = string.Empty;

	private SpinBox _scaleSpin;
	private float _currentScale = 1f;
	private Control _backButtonContainer;
	private Label _titleLabel;
	private Label _stageLabel;
	private Button _exportButton;
	private Button _loadButton;
	private Control _scalePanel;
	private TemplatePickerDialog _templatePicker;
	private TemplateSaveDialog _saveDialog;
	private LineEdit _scaleLineEdit;
	private bool _isUpdatingScaleUI;
	private string _lastValidScaleText;

	public override void _Ready() {
		base._Ready();
		_currentScale = ExportScale;
		SetGlobalShapeScale(ExportScale);
		InitializeScaleControl();
		InitializeTemplatePicker();
		InitializeSaveDialog();
		ResolveLayoutNodes();
		ArrangeHud();
	}

	private void InitializeSaveDialog()
	{
		_saveDialog = new TemplateSaveDialog();
		_saveDialog.TopLevel = true;
		_saveDialog.SetAnchorsPreset(Control.LayoutPreset.FullRect);
		_saveDialog.ZIndex = 200;
		AddChild(_saveDialog);
		
		_saveDialog.SaveConfirmed += OnSaveConfirmed;
	}

	private void OnSaveConfirmed(string filename)
	{
		SaveTemplateWithName(filename);
	}

	private void SaveTemplateWithName(string filename)
	{
		int shapeCount = GetActiveShapeCount();
		if (shapeCount == 0)
		{
			GD.Print("[TemplateBuilderStage] Tidak ada shape untuk disimpan.");
			return;
		}

		// Siapkan data untuk save ke JSON
		var entries = GetActiveShapeEntries();

		// Save ke JSON dengan nama yang dipilih user
		string folderPath = ProjectSettings.GlobalizePath("res://StagesReceipt/");
		string fullPath = System.IO.Path.Combine(folderPath, filename + ".json");

		// Pastikan folder exists
		if (!System.IO.Directory.Exists(folderPath))
		{
			System.IO.Directory.CreateDirectory(folderPath);
		}

		// Save ke JSON
		bool success = TemplateLoader.SaveToJson(
			fullPath,
			filename,
			ExportScale,
			entries
		);

		if (success)
		{
			GD.Print($"[TemplateBuilderStage] ✓ Template disimpan: {filename}.json");
			GD.Print($"[TemplateBuilderStage] Location: {fullPath}");
		}
		else
		{
			GD.PrintErr("[TemplateBuilderStage] ✗ Gagal menyimpan template");
		}
	}

	// Override ExportLayout untuk show dialog instead of auto-save
	public new void ExportLayout()
	{
		if (GetActiveShapeCount() == 0)
		{
			GD.Print("[TemplateBuilderStage] Tidak ada shape yang ditempatkan.");
			return;
		}

		_saveDialog?.ShowDialog();
	}

	private void InitializeTemplatePicker()
	{
		_templatePicker = new TemplatePickerDialog();
		_templatePicker.TopLevel = true;
		_templatePicker.SetAnchorsPreset(Control.LayoutPreset.FullRect);
		_templatePicker.ZIndex = 200;  // Set z-index tinggi agar di atas semua elemen
		AddChild(_templatePicker);
		
		_templatePicker.TemplateSelected += OnTemplateSelected;
	}

	private void OnTemplateSelected(string filePath)
	{
		LoadTemplateFromPath(filePath);
	}

	private void InitializeScaleControl() {
		_scaleSpin = GetNodeOrNull<SpinBox>(ScaleControlPath);
		if (_scaleSpin == null) return;

		_scaleSpin.MinValue = -2.0;
		_scaleSpin.MaxValue = 2.0;
		_scaleSpin.Step = 0.05;
		_scaleSpin.Value = ExportScale;
		_scaleSpin.FocusMode = Control.FocusModeEnum.None;
		_scaleSpin.ValueChanged += OnScaleValueChanged;
		_scaleLineEdit = _scaleSpin.GetLineEdit();
		if (_scaleLineEdit != null) {
			_lastValidScaleText = ExportScale.ToString("0.###", CultureInfo.InvariantCulture);
			_scaleLineEdit.Text = _lastValidScaleText;
			_scaleLineEdit.TextChanged += OnScaleLineTextChanged;
			_scaleLineEdit.TextSubmitted += OnScaleLineTextSubmitted;
		}
	}

	private void ResolveLayoutNodes() {
		_backButtonContainer = GetNodeOrNull<Control>(BackButtonContainerPath);
		_titleLabel = GetNodeOrNull<Label>(TitleLabelPath);
		_exportButton = GetNodeOrNull<Button>(ExportButtonPath);
		_loadButton = GetNodeOrNull<Button>(LoadButtonPath);
		_scalePanel = GetNodeOrNull<Control>(ScalePanelPath);
	}

	private void OnScaleValueChanged(double value) {
		value = Math.Clamp(value, -2.0, 2.0);
		float previous = _currentScale;
		ExportScale = (float)value;
		float newScale = ExportScale;
		if (!Mathf.IsZeroApprox(previous)) {
			float ratio = Mathf.IsZeroApprox(previous) ? 1f : newScale / previous;
			if (!Mathf.IsEqualApprox(ratio, 1f)) {
				var rect = GetWorkspaceRect();
				Vector2 center = rect.Position + rect.Size / 2f;
				ScaleActiveShapes(center, ratio);
			}
		}

		_currentScale = newScale;
		SetGlobalShapeScale(newScale);

		_lastValidScaleText = newScale.ToString("0.###", CultureInfo.InvariantCulture);
		if (_scaleLineEdit != null) {
			bool prevFlag = _isUpdatingScaleUI;
			_isUpdatingScaleUI = true;
			_scaleLineEdit.Text = _lastValidScaleText;
			_scaleLineEdit.CaretColumn = _scaleLineEdit.Text.Length;
			_isUpdatingScaleUI = prevFlag;
		}
	}

	public void LoadLayout() {
		string folderPath = ProjectSettings.GlobalizePath("res://StagesReceipt/");
		var files = TemplateLoader.GetAllTemplateFiles(folderPath);

		if (files.Count == 0) {
			GD.Print("[TemplateBuilderStage] Tidak ada template JSON ditemukan di StagesReceipt/");
			return;
		}

		// Show picker dialog
		_templatePicker?.ShowPicker(files);
	}

	private void LoadTemplateFromPath(string path) {
		if (string.IsNullOrEmpty(path) || !File.Exists(path)) {
			GD.PrintErr($"[TemplateBuilderStage] File tidak ditemukan: {path}");
			return;
		}

		var template = TemplateLoader.LoadFromReceipt(path);
		if (template == null) return;

		RemoveAllShapes(refundPalette: false);

		float targetScale = template.Scale.HasValue ? Mathf.Max(0.01f, template.Scale.Value) : ExportScale;
		ExportScale = targetScale;
		_currentScale = ExportScale;
		SetGlobalShapeScale(ExportScale);

		if (_scaleSpin != null) {
			_scaleSpin.ValueChanged -= OnScaleValueChanged;
			_scaleSpin.Value = ExportScale;
			_scaleSpin.ValueChanged += OnScaleValueChanged;
		}
		OnScaleValueChanged(ExportScale);

		foreach (var entry in template.Entries) {
			var instance = SpawnShapeInstance(entry.ShapeType, entry.Pivot, entry.AngleRadians, consumeQuota: false);
			if (instance != null) instance.IsLocked = false;
		}

		string filename = System.IO.Path.GetFileName(path);
		GD.Print($"[TemplateBuilderStage] ✓ Template dimuat: {filename} ({template.Entries.Count} shapes)");
		QueueRedraw();
	}

	protected override void OnViewportResized() {
		base.OnViewportResized();
		ArrangeHud();
	}

	public override void _Input(InputEvent @event) {
		if (@event is InputEventMouseButton mouseButton && mouseButton.Pressed && mouseButton.ButtonIndex == MouseButton.Left) {
			if (_scaleLineEdit != null && _scaleLineEdit.HasFocus()) {
				bool insideScale =
					(_scaleSpin != null && _scaleSpin.GetGlobalRect().HasPoint(mouseButton.Position)) ||
					(_scaleLineEdit != null && _scaleLineEdit.GetGlobalRect().HasPoint(mouseButton.Position));

				if (!insideScale) {
					CommitScaleLineEdit();
					_scaleLineEdit.ReleaseFocus();
				}
			}
		}

		if (@event is InputEventKey keyEvent && keyEvent.Pressed && !keyEvent.Echo) {
			if (IsDialogActive()) return;

			bool isRotationKey = keyEvent.Keycode == Key.Q || keyEvent.Keycode == Key.E;
			bool scaleEditing = _scaleLineEdit != null && _scaleLineEdit.HasFocus();

			if (scaleEditing && isRotationKey) {
				return;
			}

			if (scaleEditing) {
				if (keyEvent.Keycode == Key.Enter || keyEvent.Keycode == Key.KpEnter) {
					CommitScaleLineEdit();
					_scaleLineEdit.ReleaseFocus();
				}
				else if (keyEvent.Keycode == Key.Up) {
					AdjustScaleByStep(_scaleSpin?.Step ?? 0.05);
				}
				else if (keyEvent.Keycode == Key.Down) {
					AdjustScaleByStep(-(_scaleSpin?.Step ?? 0.05));
				}
				return;
			}
		}

		base._Input(@event);
	}

	private bool IsDialogActive() {
		if (_saveDialog != null && _saveDialog.Visible) return true;
		if (_templatePicker != null && _templatePicker.Visible) return true;
		return false;
	}

	private void OnScaleLineTextSubmitted(string text) {
		CommitScaleLineEdit();
	}

	private void OnScaleLineTextChanged(string newText) {
		if (_isUpdatingScaleUI) return;
		if (_scaleLineEdit == null) return;

		string normalized = string.IsNullOrEmpty(newText) ? string.Empty : newText.Replace(',', '.');
		if (normalized != newText) {
			_isUpdatingScaleUI = true;
			_scaleLineEdit.Text = normalized;
			_scaleLineEdit.CaretColumn = normalized.Length;
			_isUpdatingScaleUI = false;
			newText = normalized;
		}

		string sanitized = SanitizeNumericText(newText);
		if (sanitized != newText) {
			_isUpdatingScaleUI = true;
			_scaleLineEdit.Text = sanitized;
			_scaleLineEdit.CaretColumn = sanitized.Length;
			_isUpdatingScaleUI = false;
		}

		if (double.TryParse(sanitized, NumberStyles.Float, CultureInfo.InvariantCulture, out double value)) {
			value = Math.Clamp(value, -2.0, 2.0);
			ApplyScaleFromInput(value);
			return;
		}

		if (string.IsNullOrEmpty(sanitized) || sanitized == "-" || sanitized == "." || sanitized == "-.") {
			return;
		}

		RevertScaleLineText();
	}

	private string SanitizeNumericText(string input) {
		if (string.IsNullOrEmpty(input)) return string.Empty;

		bool hasDecimal = false;
		List<char> chars = new();

		for (int i = 0; i < input.Length; i++) {
			char c = input[i];
			if (char.IsDigit(c)) {
				chars.Add(c);
				continue;
			}

			if (c == '-' && i == 0) {
				if (!chars.Contains('-')) chars.Add('-');
				continue;
			}

			if (c == '.' && !hasDecimal) {
				hasDecimal = true;
				chars.Add('.');
				continue;
			}
		}

		if (chars.Count == 0) return string.Empty;
		if (chars.Count == 1 && chars[0] == '-') return "-";
		if (chars.Count == 1 && chars[0] == '.') return ".";
		if (chars.Count == 2 && chars[0] == '-' && chars[1] == '.') return "-.";

		return new string(chars.ToArray());
	}

	private void ApplyScaleFromInput(double value) {
		if (_scaleSpin == null) return;
		if (Math.Abs(_scaleSpin.Value - value) > 0.0001) {
			_isUpdatingScaleUI = true;
			_scaleSpin.Value = value;
			_isUpdatingScaleUI = false;
		} else {
			OnScaleValueChanged(value);
		}
	}

	private void AdjustScaleByStep(double delta) {
		if (_scaleSpin == null) return;
		double next = Math.Clamp(_scaleSpin.Value + delta, -2.0, 2.0);
		ApplyScaleFromInput(next);
		if (_scaleLineEdit != null) {
			_isUpdatingScaleUI = true;
			_scaleLineEdit.Text = next.ToString("0.###", CultureInfo.InvariantCulture);
			_scaleLineEdit.CaretColumn = _scaleLineEdit.Text.Length;
			_isUpdatingScaleUI = false;
		}
	}

	private void RevertScaleLineText() {
		if (_scaleLineEdit == null) return;
		_isUpdatingScaleUI = true;
		_scaleLineEdit.Text = _lastValidScaleText;
		_scaleLineEdit.CaretColumn = _scaleLineEdit.Text.Length;
		_isUpdatingScaleUI = false;
	}

	private void CommitScaleLineEdit() {
		if (_scaleLineEdit == null) return;
		string text = _scaleLineEdit.Text.Trim();
		if (double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out double value)) {
			value = Math.Clamp(value, -2.0, 2.0);
			ApplyScaleFromInput(value);
		} else if (string.IsNullOrEmpty(text)) {
			ApplyScaleFromInput(ExportScale);
		} else {
			RevertScaleLineText();
		}
	}

	private void ArrangeHud() {
		if (!IsInsideTree()) return;

		var viewport = GetViewport();
		if (viewport == null) return;

		Vector2 viewSize = viewport.GetVisibleRect().Size;
		float margin = Mathf.Max(24f, Mathf.Min(viewSize.X, viewSize.Y) * 0.035f);
		float scaleY = Mathf.Clamp(viewSize.Y / 720f, 0.7f, 1.6f);
		float baseLine = 34f * scaleY;

		if (_titleLabel != null) {
			float width = Mathf.Min(viewSize.X * 0.5f, viewSize.X - margin * 2f);
			float height = baseLine * 1.3f;
			_titleLabel.Size = new Vector2(width, height);
			_titleLabel.Position = new Vector2((viewSize.X - width) * 0.5f, margin * 0.25f);
		}

		if (_stageLabel != null)
			SetControlBounds(_stageLabel, new Vector2(margin, margin + baseLine * 0.4f), new Vector2(Mathf.Min(viewSize.X * 0.35f, 420f), baseLine * 1.1f));

		float controlsY = margin + baseLine * 1.9f;
		Vector2 buttonSize = new Vector2(Mathf.Clamp(viewSize.X * 0.12f, 150f, 260f), baseLine * 1.05f);

		if (_exportButton != null)
			SetControlBounds(_exportButton, new Vector2(margin, controlsY), buttonSize);

		if (_loadButton != null) {
			float spacing = 12f * scaleY;
			Vector2 position = new Vector2(margin + buttonSize.X + spacing, controlsY);
			SetControlBounds(_loadButton, position, buttonSize);
		}

		if (_scalePanel != null) {
			Vector2 minSize = _scalePanel.GetCombinedMinimumSize();
			float spacing = 12f * scaleY;
			float xStart = margin + buttonSize.X * 2f + spacing * 2f;
			float width = Mathf.Max(minSize.X, viewSize.X * 0.15f);
			float height = Mathf.Max(minSize.Y, buttonSize.Y);
			SetControlBounds(_scalePanel, new Vector2(xStart, controlsY), new Vector2(width, height));
		}

		if (_backButtonContainer != null) {
			Vector2 minSize = _backButtonContainer.GetCombinedMinimumSize();
			float width = Mathf.Max(minSize.X, Mathf.Min(viewSize.X * 0.45f, viewSize.X - margin * 2f));
			float height = Mathf.Max(minSize.Y, baseLine * 1.2f);
			_backButtonContainer.Size = new Vector2(width, height);
			_backButtonContainer.Position = new Vector2((viewSize.X - width) * 0.5f, viewSize.Y - margin - height);
		}
	}

	private void SetControlBounds(Control control, Vector2 position, Vector2 size) {
		if (control == null) return;

		control.Position = position;
		control.Size = size;
	}
}
