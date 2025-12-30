using Godot;

public partial class TemplateShapeStage : ShapePlayground {
	[Export]
	public bool ArrowAutoLayout { get; set; } = true;

	private Sprite2D _arrowSprite;
	private Vector2 _arrowInitialScale = Vector2.Zero;
	private Vector2 _arrowInitialPosition = Vector2.Zero;
	private bool _arrowInitialCaptured;

	// Tujuan: mengaitkan seluruh node HUD dan mendaftarkan handler popup/guide.
	private void ResolveHudReferences() {
		_statusLabel = ResolveStatusLabel();
		_timerLabel = ResolveTimerLabel();
		_bestTimeLabel = ResolveBestTimeLabel();
		_stageTitleLabel = ResolveStageTitleLabel();
		_completionPopup = ResolveCompletionPopup();
		_backButtonContainer = BackButtonContainerPath.IsEmpty ? null : GetNodeOrNull<Control>(BackButtonContainerPath);
		_mainTitleLabel = MainTitleLabelPath.IsEmpty ? null : GetNodeOrNull<Label>(MainTitleLabelPath);
		_arrowSprite = GetNodeOrNull<Sprite2D>("Arrow");
		if (_arrowSprite != null && !_arrowInitialCaptured) {
			_arrowInitialScale = _arrowSprite.Scale;
			_arrowInitialPosition = _arrowSprite.Position;
			_arrowInitialCaptured = true;
		}

		if (_completionPopup != null) {
			_completionPopup.HidePopupImmediate();
			_completionPopup.ReplayRequested += OnStageReplayRequested;
			_completionPopup.BackRequested += OnStageBackRequested;
			_completionPopup.ContinueRequested += OnStageContinueRequested;
		}

		_guideButton = GuideButtonPath.IsEmpty ? null : GetNodeOrNull<Button>(GuideButtonPath);
		if (_guideButton != null) _guideButton.Pressed += OnGuideButtonPressed;

		EnsureGuideOverlay();
	}

	// Tujuan: temukan label status berdasarkan NodePath atau fallback default.
	// Output: balikin Label yang relevan atau null jika tidak ketemu.
	private Label ResolveStatusLabel() {
		string targetPath = StatusLabelPath.ToString();
		if (!string.IsNullOrEmpty(targetPath)) {
			var label = GetNodeOrNull<Label>(StatusLabelPath);
			if (label != null) return label;
		}

		return GetNodeOrNull<Label>("StatusLabel");
	}

	// Tujuan: temukan label timer berdasarkan NodePath atau fallback default.
	// Output: balikin Label timer atau null jika tidak ketemu.
	private Label ResolveTimerLabel() {
		if (!TimerLabelPath.IsEmpty) {
			var label = GetNodeOrNull<Label>(TimerLabelPath);
			if (label != null) return label;
		}

		return GetNodeOrNull<Label>("TimerLabel");
	}

	// Tujuan: temukan label best time berdasarkan NodePath atau fallback default.
	// Output: balikin Label best time atau null jika tidak ketemu.
	private Label ResolveBestTimeLabel() {
		if (!BestTimeLabelPath.IsEmpty) {
			var label = GetNodeOrNull<Label>(BestTimeLabelPath);
			if (label != null) return label;
		}

		return GetNodeOrNull<Label>("BestTimeLabel");
	}

	// Tujuan: temukan label judul stage berdasarkan NodePath atau fallback default.
	// Output: balikin Label judul stage atau null jika tidak ada.
	private Label ResolveStageTitleLabel() {
		if (!StageTitleLabelPath.IsEmpty) {
			var label = GetNodeOrNull<Label>(StageTitleLabelPath);
			if (label != null) return label;
		}

		return GetNodeOrNull<Label>("StageLabel");
	}

	// Tujuan: temukan popup completion berdasarkan NodePath atau fallback default.
	// Output: balikin StageCompletionPopup atau null kalau tidak ditemukan.
	private StageCompletionPopup ResolveCompletionPopup() {
		if (!CompletionPopupPath.IsEmpty) {
			var popup = GetNodeOrNull<StageCompletionPopup>(CompletionPopupPath);
			if (popup != null) return popup;
		}

		return GetNodeOrNull<StageCompletionPopup>("StageCompletionPopup");
	}

	// Tujuan: menurunkan properti StageId dan StageDisplayName menjadi ID/nama final.
	private void ResolveStageIdentity() {
		string id = string.IsNullOrWhiteSpace(StageId) ? GetType().Name : StageId;
		_resolvedStageId = id.Trim().ToLowerInvariant();

		if (!string.IsNullOrWhiteSpace(StageDisplayName)) {
			_resolvedStageName = StageDisplayName.Trim();
		} else if (_stageTitleLabel != null && !string.IsNullOrWhiteSpace(_stageTitleLabel.Text)) {
			_resolvedStageName = _stageTitleLabel.Text.Trim();
		} else {
			_resolvedStageName = id.Trim();
		}
	}

	// Tujuan: menata ulang komponen HUD agar adaptif terhadap ukuran viewport.
	private void ArrangeHudLayout() {
		if (!IsInsideTree()) return;

		var viewport = GetViewport();
		if (viewport == null) return;

		Vector2 viewSize = viewport.GetVisibleRect().Size;
		float margin = Mathf.Max(32f, viewSize.Y * 0.045f);
		float scaleY = Mathf.Clamp(viewSize.Y / 720f, 0.7f, 1.6f);
		float baseLine = 32f * scaleY;
		float columnWidth = Mathf.Min(viewSize.X * 0.25f, 380f);

		Rect2 workArea = GetWorkspaceRect();
		float leftPadding = workArea.Position.X + Mathf.Max(16f, viewSize.X * 0.0125f);
		float currentY = workArea.Position.Y + Mathf.Max(12f, viewSize.Y * 0.017f);

		SetControlBounds(_bestTimeLabel, new Vector2(leftPadding, currentY), new Vector2(columnWidth, baseLine * 0.95f));
		currentY += baseLine * 0.7f;
		SetControlBounds(_statusLabel, new Vector2(leftPadding, currentY), new Vector2(columnWidth, baseLine * 0.95f));
		currentY += baseLine * 0.7f;
		SetControlBounds(_timerLabel, new Vector2(leftPadding, currentY), new Vector2(columnWidth, baseLine * 0.95f));

		if (_stageTitleLabel != null) {
			Rect2 workspace = GetWorkspaceRect();
			float titleWidth = Mathf.Max(300f, workspace.Size.X * 0.35f);
			float titleMargin = Mathf.Max(16f, viewSize.X * 0.0125f);
			float titleFontSize = baseLine;

			string stageName = _stageTitleLabel.Text?.Trim() ?? string.Empty;
			stageName = stageName.Replace("___", string.Empty).Trim();
			if (!string.IsNullOrEmpty(stageName)) _stageTitleLabel.Text = stageName;

			_stageTitleLabel.Size = new Vector2(titleWidth, titleFontSize * 1.2f);
			float offsetRight = titleWidth * 0.2f;
			_stageTitleLabel.Position = new Vector2(workspace.End.X - titleWidth - titleMargin + offsetRight, workspace.Position.Y + titleMargin * 0.5f);
			_stageTitleLabel.HorizontalAlignment = HorizontalAlignment.Center;
			_stageTitleLabel.RemoveThemeFontSizeOverride("font_size");
			_stageTitleLabel.AddThemeFontSizeOverride("font_size", (int)titleFontSize);

			var boldFont = ThemeDB.FallbackFont;
			if (boldFont != null) _stageTitleLabel.AddThemeFontOverride("font", boldFont);
		}

		if (_mainTitleLabel != null) _mainTitleLabel.Visible = false;

		if (_backButtonContainer != null) {
			Vector2 minSize = _backButtonContainer.GetCombinedMinimumSize();
			float buttonWidth = Mathf.Max(minSize.X, Mathf.Min(viewSize.X * 0.3f, viewSize.X - margin * 2f));
			float buttonHeight = Mathf.Max(minSize.Y, baseLine * 1.15f);
			_backButtonContainer.Size = new Vector2(buttonWidth, buttonHeight);
			_backButtonContainer.Position = new Vector2((viewSize.X - buttonWidth) * 0.5f, viewSize.Y - margin - buttonHeight);
		}

		if (_completionPopup != null) {
			_completionPopup.Position = Vector2.Zero;
			_completionPopup.Size = viewSize;
		}

		if (_arrowSprite != null) {
			if (!ArrowAutoLayout) {
				if (_arrowInitialCaptured) {
					_arrowSprite.Position = _arrowInitialPosition;
					_arrowSprite.Scale = _arrowInitialScale;
				}
			} else if (_stageTitleLabel != null) {
				Vector2 titlePos = _stageTitleLabel.Position;
				Vector2 titleSize = _stageTitleLabel.Size;
				float arrowX = titlePos.X + titleSize.X * 0.25f;
				float arrowY = titlePos.Y + titleSize.Y + Mathf.Max(20f, titleSize.Y * 1.2f);
				_arrowSprite.Position = new Vector2(arrowX, arrowY);

				float scaleMultiplier = Mathf.Clamp(titleSize.Y / 70f, 0.5f, 1.5f);
				Vector2 baseScale = _arrowInitialScale;
				if (baseScale == Vector2.Zero) baseScale = new Vector2(-0.296347f, 0.272662f);
				_arrowSprite.Scale = baseScale * scaleMultiplier;
			}
		}
	}

	// Tujuan: helper untuk memindahkan kontrol HUD ke posisi dan ukuran tertentu.
	// Input: control target Godot, position koordinat baru, size ukuran baru.
	private void SetControlBounds(Control control, Vector2 position, Vector2 size) {
		if (control == null) return;
		control.Position = position;
		control.Size = size;
	}

	// Tujuan: memperbarui label status persentase penyusunan template.
	protected void UpdateStatusLabel() {
		if (_statusLabel == null) return;

		if (_slots.Count == 0) {
			_statusLabel.Text = "Template tidak tersedia";
			return;
		}

		int matched = GetMatchedSlotCount();
		float percent = matched * 100f / _slots.Count;
		_statusLabel.Text = _templateCompleted ? $"Selesai! {percent:0.00}%" : string.Format("{0:0.00}%", percent);

		Color progressColor = percent switch {
			>= 100f => new Color(0.3f, 1f, 0.3f),
			>= 50f => new Color(1f, 0.9f, 0.2f),
			_ => new Color(0.7f, 0.7f, 0.7f)
		};
		if (_templateCompleted) progressColor = new Color(0.3f, 1f, 0.3f);

		_statusLabel.RemoveThemeColorOverride("font_color");
		_statusLabel.AddThemeColorOverride("font_color", progressColor);
	}

	// Tujuan: memastikan overlay guide tersedia dan dipasang.
	private void EnsureGuideOverlay() {
		if (_guideOverlay != null) return;

		const string overlayPath = "res://Scenes/Components/StageGuideOverlay.tscn";
		var scene = ResourceLoader.Load<PackedScene>(overlayPath);
		if (scene == null) {
			GD.PrintErr($"[TemplateShapeStage] Guide overlay scene tidak ditemukan di {overlayPath}.");
			return;
		}

		_guideOverlay = scene.Instantiate<StageGuideOverlay>();
		if (_guideOverlay == null) {
			GD.PrintErr("[TemplateShapeStage] Gagal menginstansiasi StageGuideOverlay.");
			return;
		}

		AddChild(_guideOverlay);
		_guideOverlay.TopLevel = true;
		_guideOverlay.SetAnchorsPreset(Control.LayoutPreset.FullRect);
		_guideOverlay.ZIndex = 45;
		_guideOverlay.HideGuideImmediate();
		_guideOverlay.GuideClosed += OnGuideOverlayClosed;
	}

	// Tujuan: melanjutkan timer saat overlay panduan ditutup user.
	private void OnGuideOverlayClosed() {
		ResumeTimer();
	}

	// Tujuan: buka overlay panduan ketika tombol bantuan ditekan.
	private void OnGuideButtonPressed() {
		ShowGuideOverlay();
	}

	// Tujuan: menampilkan overlay panduan sekaligus menjeda timer stage.
	private void ShowGuideOverlay() {
		EnsureGuideOverlay();
		if (_guideOverlay == null) return;

		PauseTimer();

		var info = StageCatalog.GetStageInfo(_resolvedStageId);
		if (info == null) {
			info = new StageCatalog.StageInfo {
				Id = string.IsNullOrWhiteSpace(_resolvedStageId) ? "stage" : _resolvedStageId,
				DisplayName = string.IsNullOrWhiteSpace(_resolvedStageName) ? "Stage" : _resolvedStageName,
				ScenePath = string.Empty,
				Difficulty = GetDifficultyFromStageId(),
				GuideSummary = "Susun bentuk sesuai outline abu-abu dengan bantuan kontrol standar.",
				Tips = System.Array.Empty<string>()
			};
		}

		_guideOverlay.ShowGuide(info);
	}

	// Tujuan: menyembunyikan overlay panduan.
	private void HideGuideOverlay() {
		_guideOverlay?.HideGuide();
	}

	// Tujuan: memperbarui label timer dengan format elapsed time dan warna khusus.
	private void UpdateTimerLabel() {
		if (_timerLabel == null) return;

		_timerLabel.Text = TimeFormatUtils.FormatElapsedTime(_elapsedSeconds);
		_timerLabel.RemoveThemeColorOverride("font_color");
		_timerLabel.AddThemeColorOverride("font_color", new Color(0.4f, 0.9f, 1f));
	}

	// Tujuan: menampilkan tingkat kesulitan di label best time.
	// Input: bestTimeSeconds data waktu terbaik (belum dipakai tapi disimpan agar siap jika diperlukan).
	private void UpdateBestTimeLabel(double? bestTimeSeconds) {
		if (_bestTimeLabel == null) return;

		string difficulty = GetDifficultyFromStageId();
		_bestTimeLabel.Text = difficulty;

		Color difficultyColor = difficulty switch {
			"EASY" => new Color(0.3f, 1f, 0.3f),
			"MEDIUM" => new Color(1f, 0.9f, 0.2f),
			"HARD" => new Color(1f, 0.3f, 0.3f),
			_ => Colors.White
		};

		_bestTimeLabel.RemoveThemeColorOverride("font_color");
		_bestTimeLabel.AddThemeColorOverride("font_color", difficultyColor);
	}

	// Tujuan: menentukan teks kesulitan berdasarkan ID stage yang sudah dirumuskan.
	// Output: string difficulty seperti EASY/MEDIUM/HARD atau UNKNOWN.
	private string GetDifficultyFromStageId() {
		if (_resolvedStageId == null) return "UNKNOWN";

		string id = _resolvedStageId.ToLowerInvariant();
		if (id.Contains("rocket")) return "MEDIUM";
		if (id.Contains("ufo")) return "EASY";
		if (id.Contains("astro")) return "HARD";
		return "CUSTOM PATTERN";
	}
}
