using Godot;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Matrix4x4 = System.Numerics.Matrix4x4;
using ShapeType = PatternShapeLibrary.ShapeType;

public partial class ShapePlayground {
	// Tujuan: siklus gambar utama yang nyusun workspace, shape aktif, palet, dan HUD.
	public override void _Draw() {
		DrawSetTransform(_viewOffset, 0, new Vector2(_viewScale, _viewScale));

		DrawWorkspace();
		DrawActiveShapes();

		DrawSetTransform(Vector2.Zero, 0, Vector2.One);

		DrawPalette();
		DrawHud();
	}

	// Tujuan: gambar area kerja utama sebagai kanvas shape.
	private void DrawWorkspace() {
		var rect = GetWorkspaceRect();
		DrawRect(rect, _workspaceBg, true);
		DrawRect(rect, _workspaceOutline, false, 2f);
	}

	// Tujuan: render daftar palet shape beserta preview dan stoknya.
	private void DrawPalette() {
		var font = ThemeDB.FallbackFont;
		int smallFontSize = 12;
		float gap = 10f;
		int numPaletteItems = _palette.Count;
		if (numPaletteItems == 0) return;

		float itemHeight = (_libraryRect.Size.Y - (numPaletteItems - 1) * gap) / numPaletteItems;

		for (int i = 0; i < numPaletteItems; i++) {
			var entry = _palette[i];
			var itemRect = new Rect2(
				_libraryRect.Position.X,
				_libraryRect.Position.Y + i * (itemHeight + gap),
				_libraryRect.Size.X,
				itemHeight
			);

			bool available = entry.Remaining > 0;
			DrawRect(itemRect, available ? _paletteBg : _paletteDisabledBg, true);
			DrawRect(itemRect, _paletteOutline, false, 1.5f);

			var previewCenter = itemRect.GetCenter();
			var outline = BuildPaletteOutline(entry.Shape, previewCenter);
			var fill = BuildPaletteFill(entry.Shape, previewCenter);
			var color = available ? entry.Shape.Color : entry.Shape.Color.Darkened(0.4f);

			if (fill.Length > 0) {
				var colors = Enumerable.Repeat(color, fill.Length).ToArray();
				DrawPolygon(fill, colors);
			}

			if (outline.Count > 0) GraphicsUtils.PutPixelAll(this, outline, GraphicsUtils.DrawStyle.StripStrip, color.Darkened(0.1f));

			if (font == null) continue;

			string label = entry.Quota.DisplayName;
			string stock = entry.IsUnlimited ? "∞" : $"x{entry.Remaining}";

			Vector2 labelPos = new Vector2(itemRect.Position.X + 8f, itemRect.Position.Y + 8f + smallFontSize);
			DrawString(font, labelPos, label, HorizontalAlignment.Left, itemRect.Size.X - 16f, smallFontSize, Colors.White);

			Vector2 stockPos = new Vector2(itemRect.End.X - 8f, itemRect.Position.Y + 8f + smallFontSize);
			DrawString(font, stockPos, stock, HorizontalAlignment.Right, itemRect.Size.X - 16f, smallFontSize, Colors.LightGray);
		}
	}

	// Tujuan: susun titik outline palet berdasarkan definisi shape dan pusat preview.
	// Input: shape definisi bentuk palet dan previewCenter posisi tengah tile.
	// Output: balikin list titik outline hasil transform.
	private List<Vector2> BuildPaletteOutline(PrebuiltShape shape, Vector2 previewCenter) {
		Matrix4x4 matrix = TransformasiFast.Identity();
		if (!Mathf.IsEqualApprox(_palettePreviewScale, 1f)) _transformasi.Scaling(ref matrix, _palettePreviewScale, _palettePreviewScale, shape.LocalPivot);
		_transformasi.Translation(ref matrix, previewCenter.X - shape.LocalPivot.X, previewCenter.Y - shape.LocalPivot.Y);
		return _transformasi.GetTransformPoint(matrix, shape.BasePoints);
	}

	// Tujuan: siapkan titik isi palet supaya poligon contoh bisa digambar.
	// Input: shape definisi bentuk dan previewCenter pusat tile.
	// Output: array titik isi yang sudah diproyeksikan atau kosong kalau gak ada fill.
	private Vector2[] BuildPaletteFill(PrebuiltShape shape, Vector2 previewCenter) {
		if (shape.FillVertices == null || shape.FillVertices.Length == 0) return Array.Empty<Vector2>();

		Matrix4x4 matrix = TransformasiFast.Identity();
		if (!Mathf.IsEqualApprox(_palettePreviewScale, 1f)) _transformasi.Scaling(ref matrix, _palettePreviewScale, _palettePreviewScale, shape.LocalPivot);
		_transformasi.Translation(ref matrix, previewCenter.X - shape.LocalPivot.X, previewCenter.Y - shape.LocalPivot.Y);

		var result = new Vector2[shape.FillVertices.Length];
		for (int i = 0; i < shape.FillVertices.Length; i++) {
			Vector2 pt = shape.FillVertices[i];
			System.Numerics.Vector3 temp = new System.Numerics.Vector3(pt.X, pt.Y, 1f);
			var transformed = new System.Numerics.Vector3(
				matrix.M11 * temp.X + matrix.M12 * temp.Y + matrix.M13 * temp.Z + matrix.M14,
				matrix.M21 * temp.X + matrix.M22 * temp.Y + matrix.M23 * temp.Z + matrix.M24,
				matrix.M31 * temp.X + matrix.M32 * temp.Y + matrix.M33 * temp.Z + matrix.M34
			);
			result[i] = new Vector2(transformed.X, transformed.Y);
		}

		return result;
	}

	// Tujuan: render shape yang sedang aktif berada di workspace.
	private void DrawActiveShapes() {
		foreach (var shape in _activeShapes) {
			var outline = BuildTransformedPoints(shape);
			var fill = BuildTransformedFillVertices(shape.Definition, shape.WorldPivot, shape.Rotation, _globalShapeScale);
			Color baseColor = shape.Definition.Color;
			Color drawColor = shape == _selectedShape ? baseColor.Lightened(0.2f) : baseColor;

			if (fill.Length > 0) {
				for (int i = 0; i < fill.Length; i++) fill[i] = fill[i].Round();
				var colors = Enumerable.Repeat(drawColor, fill.Length).ToArray();
				DrawPolygon(fill, colors);
			}

			if (outline.Count > 0) {
				for (int i = 0; i < outline.Count; i++) outline[i] = outline[i].Round();
				GraphicsUtils.PutPixelAll(this, outline, GraphicsUtils.DrawStyle.StripStrip, drawColor.Darkened(0.1f));
			}
		}
	}

	// Tujuan: sediakan hook menggambar elemen HUD tambahan bila perlu.
	private void DrawHud() {
		// Instruction text intentionally omitted untuk menjaga UI tetap bersih.
	}

	// Tujuan: hitung rect palet berdasarkan index supaya tooltip dan klik akurat.
	// Input: index posisi entry di dalam list palet.
	// Output: Rect2 area layar yang ditempati entry tersebut.
	private Rect2 GetPaletteRect(int index) {
		if (_palette.Count == 0) return new Rect2();
		float gap = 10f;
		float itemHeight = (_libraryRect.Size.Y - (_palette.Count - 1) * gap) / _palette.Count;
		return new Rect2(
			_libraryRect.Position.X,
			_libraryRect.Position.Y + index * (itemHeight + gap),
			_libraryRect.Size.X,
			itemHeight
		);
	}

	// Tujuan: update teks tooltip palet sesuai posisi kursor.
	// Input: screenPosition posisi kursor dalam koordinat layar.
	private void UpdatePaletteTooltip(Vector2 screenPosition) {
		string tooltip = string.Empty;
		for (int i = 0; i < _palette.Count; i++) {
			var rect = GetPaletteRect(i);
			if (!rect.HasPoint(screenPosition)) continue;

			var entry = _palette[i];
			bool isDisabled = !entry.IsUnlimited && entry.Remaining <= 0;
			tooltip = isDisabled
				? (entry.Quota.InitialMax == 0 ? "Not used in this stage" : "Semua shape sudah dipakai")
				: (entry.IsUnlimited ? string.Format("{0} – ∞", entry.Quota.DisplayName) : string.Format("{0} – x{1}", entry.Quota.DisplayName, entry.Remaining));
			break;
		}

		if (string.IsNullOrEmpty(tooltip)) {
			HidePaletteTooltip();
			return;
		}

		EnsurePaletteTooltip();
		if (_paletteTooltipLabel == null) return;

		_paletteTooltipLabel.Text = tooltip;
		_paletteTooltipLabel.Visible = true;
		_paletteTooltipLabel.Position = screenPosition + _paletteTooltipOffset;
	}

	// Tujuan: pastikan label tooltip sudah ada dan siap dipakai.
	private void EnsurePaletteTooltip() {
		if (_paletteTooltipLabel != null) return;

		var style = new StyleBoxFlat {
			BgColor = new Color(0.08f, 0.12f, 0.2f, 0.92f),
			CornerRadiusTopLeft = 6,
			CornerRadiusTopRight = 6,
			CornerRadiusBottomRight = 6,
			CornerRadiusBottomLeft = 6,
			ShadowColor = new Color(0, 0, 0, 0.35f),
			ShadowSize = 6
		};
		style.ContentMarginLeft = 6f;
		style.ContentMarginRight = 6f;
		style.ContentMarginTop = 4f;
		style.ContentMarginBottom = 4f;

		_paletteTooltipLabel = new Label {
			Name = "PaletteTooltip",
			Visible = false,
			MouseFilter = Control.MouseFilterEnum.Ignore,
			TopLevel = true
		};
		_paletteTooltipLabel.AddThemeColorOverride("font_color", Colors.White);
		_paletteTooltipLabel.AddThemeStyleboxOverride("normal", style);
		_paletteTooltipLabel.AddThemeFontSizeOverride("font_size", 14);
		_paletteTooltipLabel.FocusMode = Control.FocusModeEnum.None;
		AddChild(_paletteTooltipLabel);
	}

	// Tujuan: sembunyikan tooltip palet saat kursor keluar area.
	private void HidePaletteTooltip() {
		if (_paletteTooltipLabel == null) return;

		_paletteTooltipLabel.Visible = false;
	}
}
