using Godot;

public partial class TemplateShapeStage : ShapePlayground {
	// Tujuan: mengembalikan state session stage ke kondisi awal.
	private void ResetStageSessionState() {
		_stageSessionInitialized = false;
		_timerRunning = false;
		_elapsedSeconds = 0;
		UpdateTimerLabel();
		_completionPopup?.HidePopupImmediate();
	}

	// Tujuan: mulai sesi stage jika semua slot sudah terpasang dan data tersedia.
	private void TryStartStageSession() {
		if (_stageSessionInitialized) return;
		if (_slots.Count == 0) return;

		StageSaveService.EnsureLoaded();
		_lastRecord = StageSaveService.GetRecord(_resolvedStageId);
		UpdateBestTimeLabel(_lastRecord?.BestTimeSeconds);
		RestartTimer();
		_stageSessionInitialized = true;
	}

	// Tujuan: menyetel ulang waktu dan mengaktifkan timer.
	private void RestartTimer() {
		_elapsedSeconds = 0;
		_timerRunning = true;
		UpdateTimerLabel();
	}

	// Tujuan: menghentikan timer dan menjaga label tetap sinkron.
	private void StopTimer() {
		if (!_timerRunning) {
			UpdateTimerLabel();
			return;
		}

		_timerRunning = false;
		UpdateTimerLabel();
	}

	// Tujuan: jeda timer tanpa mengubah tampilan (dipakai saat buka guide).
	private void PauseTimer() {
		_timerRunning = false;
	}

	// Tujuan: melanjutkan timer jika sesi sudah pernah dimulai.
	private void ResumeTimer() {
		if (_stageSessionInitialized) _timerRunning = true;
	}

	// Tujuan: menghitung persentase slot yang sudah terisi.
	// Output: balikin rasio 0-1 menggambarkan akurasi penempatan.
	private double GetCompletionAccuracy() {
		int total = _slots.Count;
		if (total <= 0) return 0;
		return (double)GetMatchedSlotCount() / total;
	}

	// Tujuan: menghitung jumlah slot yang berhasil ditempati shape.
	// Output: balikin angka slot terisi.
	private int GetMatchedSlotCount() {
		int matched = 0;
		foreach (var slot in _slots) {
			if (slot.IsOccupied && slot.Occupant != null) matched++;
		}

		return matched;
	}

	// Tujuan: menangani logika saat seluruh template selesai tersusun.
	private void HandleStageCompletion() {
		StopTimer();
		double elapsed = _elapsedSeconds;
		double accuracy = GetCompletionAccuracy();
		var update = StageSaveService.RegisterCompletion(_resolvedStageId, elapsed);

		_lastRecord = new StageSaveEntry {
			BestTimeSeconds = update.BestTimeSeconds,
			Completed = true
		};

		UpdateBestTimeLabel(update.BestTimeSeconds);
		int matched = GetMatchedSlotCount();
		int total = _slots.Count;
		_completionPopup?.ShowResult(_resolvedStageName, elapsed, accuracy, update.BestTimeSeconds, update.IsNewRecord, matched, total);
	}

	// Tujuan: eksekusi saat user memilih replay pada popup kemenangan.
	private void OnStageReplayRequested() {
		HideGuideOverlay();
		RemoveAllShapes(refundPalette: false);
		LoadTemplate();
	}

	// Tujuan: menangani klik tombol kembali di popup kemenangan.
	private void OnStageBackRequested() {
		HideGuideOverlay();
		const string playScene = "res://Scenes/Pages/Projects.tscn";
		if (!string.IsNullOrEmpty(playScene)) GetTree().ChangeSceneToFile(playScene);
	}

	// Tujuan: lanjut ke stage berikutnya jika tersedia, atau tutup popup.
	private void OnStageContinueRequested() {
		HideGuideOverlay();
		var nextStage = StageCatalog.GetNextStage(_resolvedStageId);
		if (nextStage != null && !string.IsNullOrWhiteSpace(nextStage.ScenePath)) {
			// Set DynamicPatternData untuk next stage
			SetupDynamicPatternData(nextStage);
			GetTree().ChangeSceneToFile(nextStage.ScenePath);
			return;
		}

		_completionPopup?.HidePopup();
	}

	// Tujuan: setup data untuk DynamicPatternStage berdasarkan StageInfo
	private void SetupDynamicPatternData(StageCatalog.StageInfo stageInfo) {
		// Map stage ID ke template path
		string templatePath = "";
		switch (stageInfo.Id.ToLowerInvariant()) {
			case "ufo":
				templatePath = "res://StagesReceipt/UFO.json";
				break;
			case "rocket":
				templatePath = "res://StagesReceipt/Rocket.json";
				break;
			case "astronaut":
				templatePath = "res://StagesReceipt/Astronout.json";
				break;
		}

		if (!string.IsNullOrEmpty(templatePath)) {
			DynamicPatternData.TemplatePath = templatePath;
			DynamicPatternData.PatternName = stageInfo.DisplayName;
			DynamicPatternData.Difficulty = stageInfo.Difficulty;
			DynamicPatternData.ReturnScene = "res://Scenes/Pages/Projects.tscn";
		}
	}

	// Tujuan: menampilkan popup kemenangan dengan data dummy untuk debugging.
	private void ShowDebugCompletionPopup() {
		if (_completionPopup == null) return;

		HideGuideOverlay();

		double elapsed = _elapsedSeconds > 0 ? _elapsedSeconds : 92.34;
		int matched = GetMatchedSlotCount();
		int total = _slots.Count;
		double accuracy = total > 0 ? (double)matched / total : 0.0;

		StageSaveEntry record = !string.IsNullOrEmpty(_resolvedStageId)
			? StageSaveService.GetRecord(_resolvedStageId)
			: null;

		double? best = record?.BestTimeSeconds;
		bool isNewRecord = false;
		if (!best.HasValue) best = elapsed - 5.0;

		_completionPopup.ShowResult(
			string.IsNullOrWhiteSpace(_resolvedStageName) ? "Stage" : _resolvedStageName,
			elapsed,
			accuracy,
			best,
			isNewRecord,
			matched,
			total);
	}
}
