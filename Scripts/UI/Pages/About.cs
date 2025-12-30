using Godot;

public partial class About : Control {
	private const string IntroTitle = "Shape Playground - Pattern Block Activity";
	private const string SummaryText =
		"Shape Playground mengadaptasi aktivitas pattern block klasik ke dalam puzzle 2D interaktif. Pemain menyusun balok warna dengan transformasi manual untuk mengisi siluet sekaligus mempraktikkan konsep grafika komputer.";
	private const string FeaturesText =
		"- Tiga stage 720p (Rocket, UFO, Astronaut) dengan outline yang siap diisi.\n" +
		"- Kontrol drag and drop, rotasi, translasi presisi, zoom, serta snapping otomatis.\n" +
		"- HUD mencatat progres dan timer, plus Template Builder dan My Patterns untuk konten kustom.";
	private const string TechText =
		"- Godot Engine 4.x dengan dukungan C#.\n" +
		"- Platform target: Desktop (Windows, macOS, Linux).\n" +
		"- Resolusi permainan: 1280 x 720.";
	private const string DevInfoText =
		"Dikembangkan oleh Arief F-sa Wijaya (241511002) sebagai proyek Evaluasi Tengah Semester Komputer Grafik 2025 di Politeknik Negeri Bandung.";

	// Tujuan: geser background ke posisi informasi ketika halaman about dibuka.
	public override void _Ready() {
		var background = GetNode<Node>("/root/BG");
		background.Call("slide_to_anchor_y", 420.0);
		SyncCopy();
	}

	private void SyncCopy() {
		SetLabelText("PanelContainer/HBoxContainer/MarginContainer/VBoxContainer/IntroTitle", IntroTitle);
		SetLabelText("PanelContainer/HBoxContainer/MarginContainer/VBoxContainer/Summary", SummaryText);
		SetLabelText("PanelContainer/HBoxContainer/MarginContainer/VBoxContainer/Features", FeaturesText);
		SetLabelText("PanelContainer/HBoxContainer/MarginContainer/VBoxContainer/Tech", TechText);
		SetLabelText("PanelContainer/HBoxContainer/MarginContainer/VBoxContainer/DevInfo", DevInfoText);
	}

	private void SetLabelText(string nodePath, string text) {
		if (GetNodeOrNull<Label>(nodePath) is { } label) label.Text = text;
	}

	// Tujuan: kembali ke halaman welcome ketika tombol back ditekan.
	private void _on_BtnBack_pressed() {
		GetTree().ChangeSceneToFile("res://Scenes/Pages/Welcome.tscn");
	}
}
