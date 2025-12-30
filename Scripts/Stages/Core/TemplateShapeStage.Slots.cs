using Godot;
using System.Collections.Generic;
using ShapeType = PatternShapeLibrary.ShapeType;

public partial class TemplateShapeStage : ShapePlayground {
	// Tujuan: mencoba menempelkan shape ke slot template terdekat.
	// Input: instance shape yang digeser dan flag lockOnSuccess untuk mengunci bila cocok.
	// Output: true bila berhasil snap, false bila tidak.
	protected bool TrySnapToTemplate(ShapeInstance instance, bool lockOnSuccess) {
		if (_slots.Count == 0) return false;

		var currentSlot = GetAssignedSlot(instance);
		var currentPreview = GetPreviewSlot(instance);
		TemplateSlot bestSlot = null;
		float bestDist = SnapDistancePixels + 1f;
		float bestAngle = AngleToleranceDegrees + 1f;

		Vector2 pivot = instance.WorldPivot;
		float angleDeg = Mathf.RadToDeg(instance.Rotation);

		foreach (var slot in _slots) {
			if (!MatchesSlotShape(slot, instance)) continue;

			bool sameSlot = slot == currentSlot;
			if (!sameSlot && slot.IsOccupied) continue;

			float dist = pivot.DistanceTo(slot.Entry.Pivot);
			if (dist > SnapDistancePixels) continue;

			float angleDiff = GetMinimalAngleDifference(angleDeg, Mathf.RadToDeg(slot.Entry.AngleRadians), slot.Entry.ShapeType);
			if (angleDiff > AngleToleranceDegrees) continue;

			bool better = dist < bestDist - 0.001f;
			if (!better && Mathf.IsEqualApprox(dist, bestDist)) better = angleDiff < bestAngle - 0.001f;

			if (better) {
				bestDist = dist;
				bestAngle = angleDiff;
				bestSlot = slot;
			}
		}

		if (bestSlot != null) {
			if (lockOnSuccess) {
				AssignSlot(instance, bestSlot, true);
			} else {
				SetPreview(instance, bestSlot);
				instance.IsLocked = false;
			}
			return true;
		}

		if (currentSlot != null) {
			ReleaseSlot(instance);
		} else if (currentPreview != null) {
			ClearPreview(instance);
		}

		instance.IsLocked = false;
		return false;
	}

	// Tujuan: validasi apakah shape cocok dengan slot tertentu.
	// Input: slot kandidat dan instance shape yang sedang diuji.
	// Output: balikin true kalau tipe shape sesuai dengan slot.
	protected virtual bool MatchesSlotShape(TemplateSlot slot, ShapeInstance instance) => slot?.Entry.ShapeType == instance?.ShapeType;

	// Tujuan: ambil slot yang sedang ditempati shape.
	// Input: instance shape yang ingin dicek.
	// Output: TemplateSlot aktif atau null kalau belum terpasang.
	private TemplateSlot GetAssignedSlot(ShapeInstance instance) {
		if (instance == null) return null;
		_shapeToSlot.TryGetValue(instance, out var slot);
		return slot;
	}

	// Tujuan: cek slot pratinjau yang sedang diarahkan oleh shape.
	// Input: instance shape yang sedang digerakkan.
	// Output: TemplateSlot pratinjau atau null jika tidak ada.
	private TemplateSlot GetPreviewSlot(ShapeInstance instance) {
		if (instance == null) return null;
		_previewSlots.TryGetValue(instance, out var slot);
		return slot;
	}

	// Tujuan: menandai slot sebagai pratinjau ketika shape digeser mendekat.
	// Input: instance shape yang di-drag dan slot kandidat.
	private void SetPreview(ShapeInstance instance, TemplateSlot slot) {
		var current = GetPreviewSlot(instance);
		if (current == slot) {
			slot.IsPreview = true;
			return;
		}

		if (current != null) {
			current.IsPreview = false;
			_previewSlots.Remove(instance);
		}

		ClearPreviewSlot(slot);
		slot.IsPreview = true;
		_previewSlots[instance] = slot;
	}

	// Tujuan: hapus status pratinjau dari shape tertentu.
	// Input: instance shape yang mau dibersihkan preview-nya.
	private void ClearPreview(ShapeInstance instance) {
		if (instance == null) return;

		if (_previewSlots.TryGetValue(instance, out var slot)) {
			if (slot != null) slot.IsPreview = false;
			_previewSlots.Remove(instance);
		}
	}

	// Tujuan: hapus status pratinjau dari slot tertentu dan shape-shape yang mengarah padanya.
	// Input: slot target yang mau dibersihkan.
	private void ClearPreviewSlot(TemplateSlot slot) {
		if (slot == null || !slot.IsPreview) return;

		var toRemove = new List<ShapeInstance>();
		foreach (var pair in _previewSlots) {
			if (pair.Value == slot) toRemove.Add(pair.Key);
		}

		foreach (var inst in toRemove) _previewSlots.Remove(inst);
		slot.IsPreview = false;
	}

	// Tujuan: menetapkan shape ke slot template secara permanen.
	// Input: instance shape, slot tujuan, dan flag lockShape buat mengunci transform.
	private void AssignSlot(ShapeInstance instance, TemplateSlot slot, bool lockShape) {
		var current = GetAssignedSlot(instance);
		if (current != null && current != slot) ReleaseSlot(instance);

		ClearPreview(instance);
		ClearPreviewSlot(slot);
		ApplySlotTransform(instance, slot);

		slot.IsPreview = false;
		slot.IsOccupied = true;
		slot.Occupant = instance;
		_shapeToSlot[instance] = slot;

		instance.IsLocked = lockShape;
		CheckTemplateCompletion();
	}

	// Tujuan: menyamakan transform shape dengan konfigurasi slot.
	// Input: instance shape dan slot tempat ia ditempelkan.
	protected virtual void ApplySlotTransform(ShapeInstance instance, TemplateSlot slot) {
		instance.WorldPivot = slot.Entry.Pivot;
		instance.Rotation = slot.Entry.AngleRadians;
	}

	// Tujuan: melepaskan shape dari slot dan membuka slot kembali.
	// Input: instance shape yang mau dilepas.
	private void ReleaseSlot(ShapeInstance instance) {
		if (!_shapeToSlot.TryGetValue(instance, out var slot)) {
			ClearPreview(instance);
			return;
		}

		if (slot.Occupant == instance) {
			slot.Occupant = null;
			slot.IsOccupied = false;
		}

		ClearPreviewSlot(slot);
		_shapeToSlot.Remove(instance);
		ClearPreview(instance);
		instance.IsLocked = false;
		CheckTemplateCompletion();
	}

	// Tujuan: hitung selisih sudut minimal dengan mempertimbangkan simetri shape.
	// Input: playerAngleDeg sudut shape pemain, templateAngleDeg sudut target, type tipe shape.
	// Output: balikin selisih sudut terkecil dalam derajat.
	private float GetMinimalAngleDifference(float playerAngleDeg, float templateAngleDeg, ShapeType type) {
		float symmetry = GetSymmetryDegrees(type);
		float diff = WrapAngleDeg(playerAngleDeg - templateAngleDeg);
		float best = Mathf.Abs(diff);

		if (symmetry <= 0f || Mathf.IsZeroApprox(symmetry) || symmetry >= 360f) return best;

		int steps = Mathf.RoundToInt(360f / symmetry);
		for (int i = 1; i < steps; i++) {
			float alt = WrapAngleDeg(diff + symmetry * i);
			best = Mathf.Min(best, Mathf.Abs(alt));
		}

		return best;
	}

	// Tujuan: menentukan derajat simetri shape untuk toleransi rotasi.
	// Input: type jenis shape yang diuji.
	// Output: nilai derajat simetri (0 berarti bebas).
	protected virtual float GetSymmetryDegrees(ShapeType type) {
		return type switch {
			ShapeType.Segitiga => 120f,
			ShapeType.Hexagon => 60f,
			ShapeType.Persegi => 90f,
			ShapeType.JajarGenjang => 180f,
			ShapeType.JajarGenjang2 => 180f,
			ShapeType.Trapesium => 360f,
			_ => 360f
		};
	}

	// Tujuan: menormalkan sudut ke rentang -180..180 derajat.
	// Input: angle nilai sudut bebas.
	// Output: sudut ter-normalisasi di kisaran -180 hingga 180 derajat.
	private float WrapAngleDeg(float angle) => Mathf.PosMod(angle + 180f, 360f) - 180f;
}
