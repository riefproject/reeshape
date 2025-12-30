namespace Godot;

using System;

public static class ScreenUtils {
	public static int ScreenWidth { get; private set; }
	public static int ScreenHeight { get; private set; }
	public static Vector2 Center { get; private set; }

	public static int MarginLeft { get; private set; } = 50;
	public static int MarginTop { get; private set; } = 100;
	public static int MarginRight { get; private set; }
	public static int MarginBottom { get; private set; }

	public static int XMax { get; private set; }
	public static int XMin { get; private set; }
	public static int YMax { get; private set; }
	public static int YMin { get; private set; }

	// Tujuan: ambil ukuran viewport terbaru dan siapin nilai utilitas posisi layar.
	// Input: viewport adalah instance Godot yang lagi aktif.
	public static void Initialize(Viewport viewport) {
		if (viewport == null) return;

		Vector2 windowSize = viewport.GetVisibleRect().Size;
		ScreenWidth = (int)windowSize.X;
		ScreenHeight = (int)windowSize.Y;
		Center = new Vector2(ScreenWidth / 2.0f, ScreenHeight / 2.0f);

		MarginRight = ScreenWidth - MarginLeft;
		MarginBottom = ScreenHeight - MarginTop;

		XMax = MarginRight;
		XMin = MarginLeft;
		YMax = MarginTop;
		YMin = MarginBottom;
	}

	// Tujuan: ubah koordinat kartesian ke koordinat dunia Godot.
	// Input: cartesianPos adalah posisi relatif terhadap titik tengah layar.
	// Output: Vector2 posisi dunia yang bisa langsung dipakai node Godot.
	public static Vector2 CartesianToWorld(Vector2 cartesianPos) {
		return new Vector2(Center.X + cartesianPos.X, Center.Y - cartesianPos.Y);
	}

	// Tujuan: konversi posisi dunia ke koordinat kartesian pusat layar.
	// Input: worldPos sebagai koordinat global yang mau diinterpretasi ulang.
	// Output: Vector2 yang nilainya relatif terhadap titik tengah layar.
	public static Vector2 WorldToCartesian(Vector2 worldPos) {
		return new Vector2(worldPos.X - Center.X, Center.Y - worldPos.Y);
	}
}
