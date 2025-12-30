namespace Godot;

using Godot;
using System.Collections.Generic;

public static class GraphicsUtils {
	public enum DrawStyle {
		DotDot,
		DotStripDot,
		StripStrip,
		CircleDot,
		CircleStrip,
		CircleDotStrip,
		EllipseDot,
		EllipseStrip,
		EllipseDotStrip
	}

	// Tujuan: gambar satu piksel pada node target dengan opsi warna khusus.
	// Input: targetNode jadi kanvas, x dan y posisi piksel, color opsional buat warna.
	public static void PutPixel(Node2D targetNode, float x, float y, Godot.Color? color = null) {
		Godot.Color actualColor = color ?? Godot.Colors.White;
		Godot.Vector2[] points = { new Godot.Vector2(Mathf.Round(x), Mathf.Round(y)) };
		Godot.Vector2[] uvs = { Vector2.Zero, Vector2.Down, Vector2.One, Vector2.Right };

		targetNode.DrawPrimitive(points, new Godot.Color[] { actualColor }, uvs);
	}

	// Tujuan: gambar sekumpulan titik sesuai pola strip atau dot supaya gaya garis konsisten.
	// Input: targetNode sebagai node tujuan, dot kumpulan titik, style tentuin pola, color opsional, stripLength panjang strip, gap jarak antar strip.
	public static void PutPixelAll(Node2D targetNode, List<Vector2> dot, DrawStyle style = DrawStyle.DotDot, Godot.Color? color = null, int stripLength = 3, int gap = 0) {
		if (style == DrawStyle.CircleStrip || style == DrawStyle.EllipseStrip) {
			foreach (var p in dot) PutPixel(targetNode, p.X, p.Y, color);
			return;
		}

		for (int i = 0; i < dot.Count; i++) {
			float x = dot[i].X;
			float y = dot[i].Y;

			bool shouldDrawThisPoint = false;
			switch (style) {
				case DrawStyle.CircleDot:
				case DrawStyle.CircleDotStrip:
					if ((i / 8) % (gap + 1) == 0) shouldDrawThisPoint = true;
					break;

				case DrawStyle.EllipseDot:
				case DrawStyle.EllipseDotStrip:
					if ((i / 4) % (gap + 1) == 0) shouldDrawThisPoint = true;
					break;

				default:
					if (i % (gap + 1) == 0) shouldDrawThisPoint = true;
					break;
			}

			if (!shouldDrawThisPoint) continue;

			switch (style) {
				case DrawStyle.DotDot:
				case DrawStyle.CircleDot:
				case DrawStyle.EllipseDot:
					PutPixel(targetNode, x, y, color);
					break;

				case DrawStyle.StripStrip: {
					int halfLength = stripLength / 2;
					for (int j = -halfLength; j <= halfLength; j++) PutPixel(targetNode, x + j, y, color);
					break;
				}

				case DrawStyle.DotStripDot: {
					PutPixel(targetNode, x, y, color);
					int sLen = 2;
					int space = 2;
					for (int j = space; j < space + sLen; j++) PutPixel(targetNode, x - j, y, color);
					for (int j = space; j < space + sLen; j++) PutPixel(targetNode, x + j, y, color);
					break;
				}

				case DrawStyle.CircleDotStrip:
				case DrawStyle.EllipseDotStrip: {
					int halfLength = stripLength / 2;
					int step = gap > 0 ? gap : 1;
					for (int j = -halfLength; j <= halfLength; j += step) PutPixel(targetNode, x + j, y, color);
					break;
				}
			}
		}
	}
}
