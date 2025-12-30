using Godot;

public partial class TemplateShapeStage : ShapePlayground {
	// Tujuan: menambahkan shortcut khusus stage template di atas handler dasar.
	// Input: keyEvent menampung event keyboard yang lagi diproses.
	private protected override void HandleKeyInput(InputEventKey keyEvent) {
		if (keyEvent.Keycode == Key.F1) {
			if (_guideOverlay != null && _guideOverlay.Visible) HideGuideOverlay();
			else ShowGuideOverlay();
			return;
		}

		if (keyEvent.Keycode == Key.Escape && _guideOverlay != null && _guideOverlay.Visible) {
			HideGuideOverlay();
			return;
		}

		if (keyEvent.Keycode == Key.Q && keyEvent.ShiftPressed && Input.IsKeyPressed(Key.Space)) {
			ShowDebugCompletionPopup();
			return;
		}

		base.HandleKeyInput(keyEvent);
	}
}
