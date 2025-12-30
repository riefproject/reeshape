using Godot;
using System.Collections.Generic;
using ShapeType = PatternShapeLibrary.ShapeType;

public partial class TemplateShapeStage : ShapePlayground {
	// Tujuan: memuat template dari file receipt dan menyiapkan slot penyusunan.
	protected void LoadTemplate() {
		ResetStageSessionState();
		_templateCompleted = false;
		_slots.Clear();
		_shapeToSlot.Clear();
		_previewSlots.Clear();

		if (string.IsNullOrWhiteSpace(ReceiptPath)) {
			GD.PrintErr("[TemplateShapeStage] ReceiptPath belum diatur.");
			return;
		}

		string path = ReceiptPath;
		if (path.StartsWith("res://")) path = ProjectSettings.GlobalizePath(path);

		_template = TemplateLoader.LoadFromReceipt(path);
		if (_template == null) {
			GD.PrintErr($"[TemplateShapeStage] Gagal memuat template {ReceiptPath}");
			return;
		}

		if (_template.Scale.HasValue) TemplateScale = Mathf.Max(0.01f, _template.Scale.Value);
		foreach (var entry in _template.Entries) _slots.Add(new TemplateSlot { Entry = entry });

		ApplyTemplateTransformations();
		QueueRedraw();
		OnTemplateLoaded();
		TryStartStageSession();
		ArrangeHudLayout();
	}

	// Tujuan: menyesuaikan pivot dan skala template agar pas dengan workspace.
	private void ApplyTemplateTransformations() {
		if (_slots.Count == 0) return;

		float scale = Mathf.Max(TemplateScale, 0.01f);
		Vector2 min = new(float.MaxValue, float.MaxValue);
		Vector2 max = new(float.MinValue, float.MinValue);

		foreach (var slot in _slots) {
			var pivot = slot.Entry.Pivot;
			if (pivot.X < min.X) min.X = pivot.X;
			if (pivot.Y < min.Y) min.Y = pivot.Y;
			if (pivot.X > max.X) max.X = pivot.X;
			if (pivot.Y > max.Y) max.Y = pivot.Y;
		}

		if (float.IsInfinity(min.X) || float.IsInfinity(min.Y)) {
			_templateScaleApplied = 1f;
			SnapDistancePixels = _snapDistanceBase;
			return;
		}

		Vector2 templateCenter = new((min.X + max.X) / 2f, (min.Y + max.Y) / 2f);
		Vector2 targetCenter = TemplateAutoCenter
			? GetWorkspaceRect().Position + GetWorkspaceRect().Size / 2f + TemplateOffset
			: templateCenter + TemplateOffset;

		foreach (var slot in _slots) {
			Vector2 relative = slot.Entry.Pivot - templateCenter;
			slot.Entry.Pivot = relative + targetCenter;
		}

		_templateScaleApplied = scale;
		SnapDistancePixels = _snapDistanceBase * scale;
		SetGlobalShapeScale(scale);
	}

	// Tujuan: mengecek apakah seluruh slot template sudah terisi.
	private void CheckTemplateCompletion() {
		if (_slots.Count == 0) {
			SetTemplateCompleted(false);
			return;
		}

		bool complete = true;
		foreach (var slot in _slots) {
			if (!slot.IsOccupied || slot.Occupant == null) {
				complete = false;
				break;
			}
		}

		SetTemplateCompleted(complete);
	}

	// Tujuan: memperbarui status selesai template dan memicu handler terkait.
	// Input: completed flag status baru hasil pengecekan.
	private void SetTemplateCompleted(bool completed) {
		if (_templateCompleted == completed) return;
		_templateCompleted = completed;
		OnTemplateCompletionChanged(completed);
	}

	// Tujuan: respon standar ketika status selesai template berubah.
	// Input: completed menandakan apakah template sudah tuntas.
	protected virtual void OnTemplateCompletionChanged(bool completed) {
		UpdateStatusLabel();

		if (!_stageSessionInitialized) {
			if (completed) GD.Print("[TemplateShapeStage] Template complete!");
			return;
		}

		if (completed) {
			GD.Print("[TemplateShapeStage] Template complete!");
			HandleStageCompletion();
		} else {
			_completionPopup?.HidePopupImmediate();
			RestartTimer();
		}
	}

	// Tujuan: menyusun definisi stage berdasarkan jumlah shape klasik.
	// Input: counts berisi quota per shape, paletteOrder menentukan urutan penambahan.
	// Output: ShapeStageDefinition hasil kompilasi kuota.
	protected ShapeStageDefinition CreateStageDefinitionFromCounts(Dictionary<ShapeType, int> counts, IEnumerable<(ShapeType type, string label)> paletteOrder) {
		var stage = new ShapeStageDefinition {
			UnitSize = Mathf.Max(1, Mathf.RoundToInt(TemplateUnitSize)),
			RotationStepDegrees = TemplateRotationStep,
			PaletteSpacing = TemplatePaletteSpacing,
			PaletteStart = TemplatePaletteStart,
			PaletteTileSize = TemplatePaletteTileSize,
			PalettePreviewScale = TemplatePalettePreviewScale
		};

		var remaining = counts != null ? new Dictionary<ShapeType, int>(counts) : new Dictionary<ShapeType, int>();

		if (paletteOrder != null) {
			foreach (var (type, label) in paletteOrder) {
				remaining.TryGetValue(type, out int limit);
				stage.AddQuota(type, limit, label);
				remaining.Remove(type);
			}
		}

		foreach (var pair in remaining) stage.AddQuota(pair.Key, pair.Value, pair.Key.ToString());
		return stage;
	}
}
