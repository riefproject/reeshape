using Godot;
using System.Linq;

public partial class StageGuideOverlay : Control {
	[Signal]
	public delegate void GuideClosedEventHandler();

	[Export]
	public NodePath TitleLabelPath { get; set; } = new NodePath("Panel/Margin/VBox/Title");

	[Export]
	public NodePath SummaryLabelPath { get; set; } = new NodePath("Panel/Margin/VBox/GameplayText");

	[Export]
	public NodePath ControlsListPath { get; set; } = new NodePath("Panel/Margin/VBox/ControlsList");

	[Export]
	public NodePath CloseButtonPath { get; set; } = new NodePath("Panel/Margin/VBox/Buttons/CloseButton");

	private const string GuideTitle = "GUIDE";
	private const string GameplayText =
		"Susun shape di library (sidebar kanan) ke outline abu-abu di kiri sampai bentuk cocok. " +
		"Gunakan mouse untuk drag & drop, keyboard untuk rotasi dan gerakan presisi.";

	private static readonly string[] ControlsList = {
		"[Left Click]\t\t\tPilih dan geser shape",
		"[WASD / Arrow Keys]\t\tGerakkan shape dengan presisi",
		"[Q / E]\t\t\t\tRotasi shape (searah/berlawanan jarum jam)",
		"[Delete / Right Click]\t\tHapus shape (kembalikan ke library)"
	};

	private Label _titleLabel;
	private Label _summaryLabel;
	private RichTextLabel _controlsList;
	private Button _closeButton;
	private Tween _activeTween;

	// Tujuan: inisialisasi komponen overlay dan pasang handler tombol tutup.
	public override void _Ready() {
		_titleLabel = GetNodeOrNull<Label>(TitleLabelPath);
		_summaryLabel = GetNodeOrNull<Label>(SummaryLabelPath);
		_controlsList = GetNodeOrNull<RichTextLabel>(ControlsListPath);
		_closeButton = GetNodeOrNull<Button>(CloseButtonPath);

		if (_closeButton != null) _closeButton.Pressed += HideGuide;

		PopulateControlsList();
		HideGuideImmediate();
	}

	// Tujuan: tampilkan overlay panduan dengan teks umum yang konsisten.
	// Input: stageInfo disediakan untuk konsistensi API walau konten tetap generik.
	public void ShowGuide(StageCatalog.StageInfo stageInfo) {
		if (_titleLabel != null) _titleLabel.Text = GuideTitle;
		if (_summaryLabel != null) _summaryLabel.Text = GameplayText;

		Visible = true;
		MouseFilter = MouseFilterEnum.Stop;
		Modulate = new Color(1f, 1f, 1f, 0f);

		_activeTween?.Kill();
		_activeTween = CreateTween();
		_activeTween.TweenProperty(this, "modulate:a", 1f, 0.25f)
			.SetTrans(Tween.TransitionType.Cubic)
			.SetEase(Tween.EaseType.Out);
	}

	// Tujuan: sembunyikan overlay dengan animasi fade-out lalu emit sinyal selesai.
	public void HideGuide() {
		if (!Visible) return;

		_activeTween?.Kill();
		_activeTween = CreateTween();
		_activeTween.TweenProperty(this, "modulate:a", 0f, 0.18f)
			.SetTrans(Tween.TransitionType.Cubic)
			.SetEase(Tween.EaseType.In);
		_activeTween.Finished += HideGuideImmediate;
		_activeTween.Finished += () => EmitSignal(SignalName.GuideClosed);
	}

	// Tujuan: langsung sembunyikan overlay tanpa animasi dan jaga state konsisten.
	public void HideGuideImmediate() {
		_activeTween?.Kill();
		bool wasVisible = Visible;
		Visible = false;
		MouseFilter = MouseFilterEnum.Ignore;
		Modulate = new Color(1f, 1f, 1f, 0f);

		if (wasVisible) EmitSignal(SignalName.GuideClosed);
	}

	// Tujuan: isi daftar kontrol dengan bullet sederhana supaya mudah dibaca.
	private void PopulateControlsList() {
		if (_controlsList == null) return;

		var bulletLines = ControlsList.Select(t => "  • " + t);
		_controlsList.Text = string.Join('\n', bulletLines);
	}
}
