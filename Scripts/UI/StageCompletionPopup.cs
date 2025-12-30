using Godot;

public partial class StageCompletionPopup : Control {
	[Signal]
	public delegate void ReplayRequestedEventHandler();

	[Signal]
	public delegate void BackRequestedEventHandler();

	[Signal]
	public delegate void ContinueRequestedEventHandler();

	[Export]
	public NodePath PanelPath { get; set; } = new NodePath("Panel");

	[Export]
	public NodePath TitleLabelPath { get; set; } = new NodePath("Panel/Margin/VBox/TitleLabel");

	[Export]
	public NodePath SubtitleLabelPath { get; set; } = new NodePath("Panel/Margin/VBox/SubtitleLabel");

	[Export]
	public NodePath TimeValueLabelPath { get; set; } = new NodePath("Panel/Margin/VBox/MetricsPanel/MetricsMargin/Metrics/MetricTime/MetricTimeMargin/MetricTimeVBox/TimeValue");

	[Export]
	public NodePath BestValueLabelPath { get; set; } = new NodePath("Panel/Margin/VBox/MetricsPanel/MetricsMargin/Metrics/MetricBest/MetricBestMargin/MetricBestVBox/BestValue");

	[Export]
	public NodePath AccuracyValueLabelPath { get; set; } = new NodePath("Panel/Margin/VBox/MetricsPanel/MetricsMargin/Metrics/MetricAccuracy/MetricAccuracyMargin/MetricAccuracyVBox/AccuracyValue");

	[Export]
	public NodePath RecordLabelPath { get; set; } = new NodePath("Panel/Margin/VBox/RecordLabel");

	[Export]
	public NodePath ReplayButtonPath { get; set; } = new NodePath("Panel/Margin/VBox/Buttons/ReplayButton");

	[Export]
	public NodePath CloseButtonPath { get; set; } = new NodePath("Panel/Margin/VBox/Buttons/CloseButton");

	[Export]
	public NodePath BackButtonPath { get; set; } = new NodePath("Panel/Margin/VBox/Buttons/BackButton");

	[Export]
	public NodePath StatsSummaryLabelPath { get; set; } = new NodePath("Panel/Margin/VBox/StatsSummary");

	[Export]
	public NodePath MetricsPanelPath { get; set; } = new NodePath("Panel/Margin/VBox/MetricsPanel");

	private Control _panel;
	private Label _titleLabel;
	private Label _subtitleLabel;
	private Label _timeValueLabel;
	private Label _bestValueLabel;
	private Label _accuracyValueLabel;
	private Label _recordLabel;
	private Label _statsSummaryLabel;
	private Button _replayButton;
	private Button _closeButton;
	private Button _backButton;
	private Control _metricsPanel;

	private Tween _activeTween;

	// Tujuan: menyiapkan referensi kontrol popup dan hubungkan event tombol saat popup siap.
	public override void _Ready() {
		TopLevel = true;
		SetAnchorsPreset(LayoutPreset.FullRect);
		ZIndex = Mathf.Max(ZIndex, 40);

		_panel = GetNodeOrNull<Control>(PanelPath);
		_titleLabel = GetNodeOrNull<Label>(TitleLabelPath);
		_subtitleLabel = GetNodeOrNull<Label>(SubtitleLabelPath);
		_timeValueLabel = GetNodeOrNull<Label>(TimeValueLabelPath);
		_bestValueLabel = GetNodeOrNull<Label>(BestValueLabelPath);
		_accuracyValueLabel = GetNodeOrNull<Label>(AccuracyValueLabelPath);
		_recordLabel = GetNodeOrNull<Label>(RecordLabelPath);
		_statsSummaryLabel = GetNodeOrNull<Label>(StatsSummaryLabelPath);
		_replayButton = GetNodeOrNull<Button>(ReplayButtonPath);
		_closeButton = GetNodeOrNull<Button>(CloseButtonPath);
		_backButton = GetNodeOrNull<Button>(BackButtonPath);
		_metricsPanel = GetNodeOrNull<Control>(MetricsPanelPath);

		if (_replayButton != null) _replayButton.Pressed += OnReplayButtonPressed;
		if (_closeButton != null) _closeButton.Pressed += OnCloseButtonPressed;
		if (_backButton != null) _backButton.Pressed += OnBackButtonPressed;

		HidePopupImmediate();
	}

	// Tujuan: tampilkan popup dengan data statistik stage dan animasi singkat.
	// Input: stageName, elapsedSeconds, accuracy01, bestTimeSeconds, isNewRecord, matchedCount, totalCount.
	public void ShowResult(string stageName, double elapsedSeconds, double accuracy01, double? bestTimeSeconds, bool isNewRecord, int matchedCount, int totalCount) {
		if (_panel == null) {
			GD.PrintErr("[StageCompletionPopup] Panel node tidak ditemukan.");
			return;
		}

		string resolvedName = string.IsNullOrWhiteSpace(stageName) ? "Stage" : stageName.Trim();
		if (_titleLabel != null) _titleLabel.Text = $"{resolvedName} selesai!";

		if (_subtitleLabel != null) {
			string accuracyText = FormatAccuracy(accuracy01, matchedCount, totalCount);
			_subtitleLabel.Text = accuracy01 >= 1.0
				? "Seluruh outline terpenuhi. Kerja bagus!"
				: $"Akurasi: {accuracyText}";
		}

		if (_timeValueLabel != null) _timeValueLabel.Text = TimeFormatUtils.FormatElapsedTime(elapsedSeconds);
		if (_bestValueLabel != null) _bestValueLabel.Text = TimeFormatUtils.FormatElapsedTime(bestTimeSeconds, "--");
		if (_accuracyValueLabel != null) _accuracyValueLabel.Text = FormatAccuracy(accuracy01, matchedCount, totalCount);

		if (_recordLabel != null) {
			if (isNewRecord) {
				_recordLabel.Visible = true;
				_recordLabel.Text = "Rekor baru!";
			} else {
				_recordLabel.Visible = false;
				_recordLabel.Text = string.Empty;
			}
		}

		if (_statsSummaryLabel != null) {
			string completedTime = TimeFormatUtils.FormatElapsedTime(elapsedSeconds);
			string recordTime = TimeFormatUtils.FormatElapsedTime(bestTimeSeconds, "--:--");
			_statsSummaryLabel.Text = $"Waktu selesai: {completedTime}\nRekor tercepat: {recordTime}";
		}

		if (_metricsPanel != null) _metricsPanel.Visible = false;
		if (_replayButton != null) _replayButton.GrabFocus();

		Visible = true;
		MouseFilter = MouseFilterEnum.Stop;
		Modulate = new Color(1f, 1f, 1f, 0f);
		if (_panel != null) _panel.Scale = new Vector2(0.9f, 0.9f);

		_activeTween?.Kill();
		_activeTween = CreateTween();
		_activeTween.TweenProperty(this, "modulate:a", 1f, 0.3f)
			.SetTrans(Tween.TransitionType.Cubic)
			.SetEase(Tween.EaseType.Out);
		if (_panel != null) {
			_activeTween.Parallel().TweenProperty(_panel, "scale", Vector2.One, 0.3f)
				.SetTrans(Tween.TransitionType.Back)
				.SetEase(Tween.EaseType.Out);
		}
	}

	// Tujuan: sembunyikan popup dengan animasi fade-out.
	public void HidePopup() {
		if (!Visible) return;

		_activeTween?.Kill();
		_activeTween = CreateTween();
		_activeTween.TweenProperty(this, "modulate:a", 0f, 0.2f)
			.SetTrans(Tween.TransitionType.Cubic)
			.SetEase(Tween.EaseType.In);
		_activeTween.Finished += () => {
			Visible = false;
			MouseFilter = MouseFilterEnum.Ignore;
		};
	}

	// Tujuan: langsung menyembunyikan popup tanpa animasi.
	public void HidePopupImmediate() {
		_activeTween?.Kill();
		Visible = false;
		MouseFilter = MouseFilterEnum.Ignore;
		Modulate = new Color(1f, 1f, 1f, 0f);
		if (_panel != null) _panel.Scale = Vector2.One;
		if (_recordLabel != null) {
			_recordLabel.Visible = false;
			_recordLabel.Text = string.Empty;
		}
		if (_statsSummaryLabel != null) _statsSummaryLabel.Text = string.Empty;
		if (_metricsPanel != null) _metricsPanel.Visible = true;
	}

	// Tujuan: handler ketika tombol close ditekan, meneruskan ke lanjutan stage.
	private void OnCloseButtonPressed() {
		HidePopup();
		EmitSignal(SignalName.ContinueRequested);
	}

	// Tujuan: handler tombol replay untuk mengulang stage.
	private void OnReplayButtonPressed() {
		HidePopup();
		EmitSignal(SignalName.ReplayRequested);
	}

	// Tujuan: handler tombol back yang membawa pemain kembali ke menu.
	private void OnBackButtonPressed() {
		HidePopup();
		EmitSignal(SignalName.BackRequested);
	}

	// Tujuan: format nilai akurasi ke teks persentase sekaligus progress matched/total.
	// Input: accuracy01 rasio 0-1, matched jumlah slot terisi, total jumlah slot.
	// Output: string akurasi siap tampil.
	private static string FormatAccuracy(double accuracy01, int matched, int total) {
		double clamped = Mathf.Clamp((float)accuracy01, 0f, 1f);
		if (total <= 0) return string.Format("{0:0.#}% (0/0)", clamped * 100.0);

		matched = Mathf.Clamp(matched, 0, total);
		return string.Format("{0:0.#}% ({1}/{2})", clamped * 100.0, matched, total);
	}
}
