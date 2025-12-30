using Godot;

public partial class Welcome : Control {
    private Control _settingsWindow;
    private Button _resetDataButton;
    private Button _closeSettingsButton;
    private Button _openGuideButton;
    private Label _resetStatusLabel;
    private Control _resetConfirmOverlay;
    private Button _confirmResetButton;
    private Button _cancelResetButton;
    private VBoxContainer _progressList;

    // Tujuan: siapkan background halaman welcome dan load progres stage.
    public override void _Ready() {
        var background = GetNodeOrNull<Node>("/root/BG");
        background?.Call("slide_to_anchor_y", 341.0);

        StageSaveService.EnsureLoaded();
        InitializeSettingsWindow();
    }

    // Tujuan: ambil referensi kontrol di window pengaturan dan pasang handler tombol.
    private void InitializeSettingsWindow() {
        _settingsWindow = GetNodeOrNull<Control>("SettingsWindow");
        if (_settingsWindow == null) return;

        _settingsWindow.Visible = false;

        _resetDataButton = _settingsWindow.GetNodeOrNull<Button>("Panel/MarginContainer/VBox/Actions/ResetDataButton");
        _closeSettingsButton = _settingsWindow.GetNodeOrNull<Button>("Panel/MarginContainer/VBox/Actions/CloseSettingsButton");
        _openGuideButton = _settingsWindow.GetNodeOrNull<Button>("Panel/MarginContainer/VBox/Actions/OpenGuideButton");
        _resetStatusLabel = _settingsWindow.GetNodeOrNull<Label>("Panel/MarginContainer/VBox/Actions/ResetStatusLabel");
        _resetConfirmOverlay = _settingsWindow.GetNodeOrNull<Control>("ResetConfirmOverlay");
        _progressList = _settingsWindow.GetNodeOrNull<VBoxContainer>("Panel/MarginContainer/VBox/ProgressPanel/ProgressMargin/ProgressVBox/ProgressScroll/ProgressList");

        if (_resetConfirmOverlay != null) {
            _resetConfirmOverlay.Visible = false;
            _confirmResetButton = _resetConfirmOverlay.GetNodeOrNull<Button>("Panel/MarginContainer/VBox/Buttons/ConfirmButton");
            _cancelResetButton = _resetConfirmOverlay.GetNodeOrNull<Button>("Panel/MarginContainer/VBox/Buttons/CancelButton");

            if (_confirmResetButton != null) _confirmResetButton.Pressed += OnResetConfirmed;
            if (_cancelResetButton != null) _cancelResetButton.Pressed += OnCancelReset;
        }

        if (_resetDataButton != null) _resetDataButton.Pressed += OnResetDataButtonPressed;
        if (_closeSettingsButton != null) _closeSettingsButton.Pressed += OnCloseSettingsButtonPressed;
        if (_openGuideButton != null) _openGuideButton.Pressed += OpenGuidePage;
        if (_resetStatusLabel != null) _resetStatusLabel.Text = string.Empty;

        RefreshProgressList();
    }

    // Tujuan: buka halaman About ketika tombol about ditekan.
    private void _on_BtnAbout_pressed() {
        GetTree().ChangeSceneToFile("res://Scenes/Pages/About.tscn");
    }

    // Tujuan: buka halaman Play ketika tombol PLAY ditekan.
    private void _on_BtnPlay_pressed() {
        GetTree().ChangeSceneToFile("res://Scenes/Pages/Projects.tscn");
    }

    // Tujuan: buka halaman guide ketika tombol guide ditekan.
    private void _on_BtnGuide_pressed() {
        OpenGuidePage();
    }

    // Tujuan: tampilkan window pengaturan dan muat ulang data progres.
    private void _on_BtnSettings_pressed() {
        if (_settingsWindow == null) return;

        _settingsWindow.Visible = true;
        _settingsWindow.MouseFilter = Control.MouseFilterEnum.Stop;
        if (_resetStatusLabel != null) _resetStatusLabel.Text = string.Empty;
        RefreshProgressList();
    }

    // Tujuan: tampilkan overlay konfirmasi reset progres.
    private void OnResetDataButtonPressed() {
        if (_resetConfirmOverlay == null) return;

        _resetConfirmOverlay.Visible = true;
        _resetConfirmOverlay.MouseFilter = Control.MouseFilterEnum.Stop;
    }

    // Tujuan: batalkan aksi reset dan tutup overlay konfirmasi.
    private void OnCancelReset() {
        if (_resetConfirmOverlay == null) return;

        _resetConfirmOverlay.Visible = false;
        _resetConfirmOverlay.MouseFilter = Control.MouseFilterEnum.Ignore;
    }

    // Tujuan: benar-benar reset progres pemain dan update tampilan status.
    private void OnResetConfirmed() {
        StageSaveService.Reset();
        if (_resetStatusLabel != null) _resetStatusLabel.Text = "Progress berhasil di-reset.";
        RefreshProgressList();

        if (_resetConfirmOverlay != null) {
            _resetConfirmOverlay.Visible = false;
            _resetConfirmOverlay.MouseFilter = Control.MouseFilterEnum.Ignore;
        }
    }

    // Tujuan: menutup window pengaturan dan kembalikan kontrol ke halaman utama.
    private void OnCloseSettingsButtonPressed() {
        if (_settingsWindow == null) return;

        _settingsWindow.Visible = false;
        _settingsWindow.MouseFilter = Control.MouseFilterEnum.Ignore;
    }

    // Tujuan: keluar dari aplikasi ketika tombol exit ditekan.
    private void _on_BtnExit_pressed() {
        GetTree().Quit();
    }

    // Tujuan: refresh list progres stage agar menampilkan status terbaru.
    private void RefreshProgressList() {
        if (_progressList == null) return;

        foreach (Node child in _progressList.GetChildren()) child.QueueFree();

        foreach (var stage in StageCatalog.ChallengeStages) {
            var row = new HBoxContainer {
                SizeFlagsHorizontal = Control.SizeFlags.ExpandFill
            };
            row.AddThemeConstantOverride("separation", 12);

            var nameLabel = new Label {
                Text = stage.DisplayName,
                SizeFlagsHorizontal = Control.SizeFlags.ExpandFill
            };

            var difficultyLabel = new Label {
                Text = stage.Difficulty,
                HorizontalAlignment = HorizontalAlignment.Center,
                SizeFlagsHorizontal = Control.SizeFlags.ShrinkCenter
            };
            difficultyLabel.AddThemeColorOverride("font_color", GetDifficultyColor(stage.Difficulty));

            StageSaveEntry record = StageSaveService.GetRecord(stage.Id);
            bool completed = record != null && record.Completed;
            double? bestSeconds = record?.BestTimeSeconds;
            string statusText = completed
                ? $"Rekor: {TimeFormatUtils.FormatElapsedTime(bestSeconds, "--:--")}" : "Belum dimainkan";

            var statusLabel = new Label {
                Text = statusText,
                HorizontalAlignment = HorizontalAlignment.Right,
                SizeFlagsHorizontal = Control.SizeFlags.ShrinkEnd
            };

            if (!completed) statusLabel.AddThemeColorOverride("font_color", new Color(0.75f, 0.78f, 0.84f, 1f));

            row.AddChild(nameLabel);
            row.AddChild(difficultyLabel);
            row.AddChild(statusLabel);
            _progressList.AddChild(row);
        }

        if (_progressList.GetChildCount() == 0) {
            _progressList.AddChild(new Label {
                Text = "Belum ada data progress stage.",
                HorizontalAlignment = HorizontalAlignment.Center
            });
        }
    }

    // Tujuan: pilih warna teks berdasarkan tingkat kesulitan stage.
    // Input: difficulty string label kesulitan.
    // Output: Color yang dipakai untuk label difficulty.
    private static Color GetDifficultyColor(string difficulty) {
        return difficulty switch {
            "EASY" => new Color(0.3f, 1f, 0.3f),
            "MEDIUM" => new Color(1f, 0.9f, 0.2f),
            "HARD" => new Color(1f, 0.3f, 0.3f),
            _ => new Color(0.85f, 0.9f, 0.95f)
        };
    }

    // Tujuan: navigasi ke halaman guide utama.
    private void OpenGuidePage() {
        GetTree().ChangeSceneToFile("res://Scenes/Pages/Guide.tscn");
    }
}
