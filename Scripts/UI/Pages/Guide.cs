using Godot;
using System.Collections.Generic;

public partial class Guide : Control {
	private struct GuidePage {
		public string Title;
		public string SubTitle;
		public string Point;
		public string Desc;
	}

	private readonly List<GuidePage> _guidePages = new();
	private int _currentPageIndex = 0;

	private Label _titleLabel;
	private Label _subTitleLabel;
	private Label _pointLabel;
	private Label _descLabel;

	// Tujuan: isi daftar konten panduan, ambil referensi UI, dan tampilkan halaman awal.
	public override void _Ready() {
		var background = GetNode<Node>("/root/BG");
		background.Call("slide_to_anchor_y", 420.0);

		PopulateGuidePages();

		_titleLabel = GetNode<Label>("HBoxContainer2/Label");
		_subTitleLabel = GetNode<Label>("VBoxContainer/SubTitle");
		_pointLabel = GetNode<Label>("VBoxContainer/Point");
		_descLabel = GetNode<Label>("VBoxContainer/Desc");

		var prevButton = GetNode<Button>("HBoxContainer2/PrevButton");
		var nextButton = GetNode<Button>("HBoxContainer2/NextButton");
		prevButton.Pressed += OnPrevButtonPressed;
		nextButton.Pressed += OnNextButtonPressed;

		UpdateGuideContent();
	}

	// Tujuan: isi daftar konten panduan dengan halaman-halaman informatif.
	private void PopulateGuidePages() {
	
	
		_guidePages.Clear();

		_guidePages.Add(new GuidePage {
			Title = "Kontrol Dasar",
			SubTitle = "Manipulasi shape",
			Point = "- Klik shape di palet untuk memunculkannya lalu drag ke siluet.\n- Putar dengan Q / E, translasi presisi pakai WASD atau tombol arah.\n- Scroll untuk zoom, tahan klik tengah (MMB) untuk pan viewport.\n- Klik kanan menghapus shape dan mengembalikannya ke palet.",
			Desc = "Tekan tombol [Guide] atau F1 di dalam stage kapan pun untuk membuka ringkasan kontrol beserta tips khusus tantangan."
		});

		_guidePages.Add(new GuidePage {
			Title = "Snapping & Progres",
			SubTitle = "Feedback permainan",
			Point = "- Shape otomatis snap saat mendekati slot outline yang tepat.\n- HUD menampilkan timer, progres %, dan label kesulitan stage.\n- Rekor waktu terbaik tersimpan otomatis di user://save.dat.\n- Popup kemenangan muncul ketika progres mencapai 100%.",
			Desc = "Gunakan indikator progres untuk mengatur tempo permainan dan ulangi stage guna memperbaiki catatan waktu."
		});

		_guidePages.Add(new GuidePage {
			Title = "Navigasi PLAY",
			SubTitle = "Memilih mode bermain",
			Point = "- Menu [PLAY] memuat tantangan Rocket, UFO, dan Astronaut.\n- [Template Builder] dipakai membuat pola baru lalu ekspor ke JSON.\n- [My Patterns] menampilkan koleksi template kustom untuk dimainkan ulang.\n- Tombol [<<] dan [>>] di pojok atas halaman ini mengganti panduan yang ditampilkan.",
			Desc = "Mulailah dari halaman Welcome untuk melihat progres teranyar sebelum memilih stage berikutnya."
		});
	}

	// Tujuan: perbarui label UI agar menampilkan halaman panduan sesuai index sekarang.
	private void UpdateGuideContent() {
		if (_guidePages.Count == 0) return;

		GuidePage currentPage = _guidePages[_currentPageIndex];
		_titleLabel.Text = currentPage.Title;
		_subTitleLabel.Text = currentPage.SubTitle;
		_pointLabel.Text = currentPage.Point;
		_descLabel.Text = currentPage.Desc;
	}

	// Tujuan: pindah ke halaman panduan sebelumnya dengan wrap-around.
	private void OnPrevButtonPressed() {
		if (_guidePages.Count == 0) return;
		_currentPageIndex = (_currentPageIndex - 1 + _guidePages.Count) % _guidePages.Count;
		UpdateGuideContent();
	}

	// Tujuan: pindah ke halaman panduan berikutnya dengan wrap-around.
	private void OnNextButtonPressed() {
		if (_guidePages.Count == 0) return;
		_currentPageIndex = (_currentPageIndex + 1) % _guidePages.Count;
		UpdateGuideContent();
	}

	// Tujuan: kembali ke halaman welcome dari guide.
	private void _on_BtnBack_pressed() {
		GetTree().ChangeSceneToFile("res://Scenes/Pages/Welcome.tscn");
	}
}
