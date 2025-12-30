using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public sealed partial class PatternShapeLibrary : RefCounted {
	public enum ShapeType {
		Persegi,
		Segitiga,
		Trapesium,
		JajarGenjang,
		JajarGenjangFlip,
		JajarGenjang2,
		JajarGenjang2Flip,
		Hexagon
	}

	public sealed class ShapeData {
		public List<Vector2> OutlinePoints { get; init; } = new();
		public Vector2[] FillVertices { get; init; } = Array.Empty<Vector2>();
		public Color Color { get; init; } = Colors.White;
		public Vector2 Pivot { get; init; } = Vector2.Zero;
	}

	private readonly BentukDasar _bentukDasar = new BentukDasar();

	public float UnitSize { get; set; } = 50f;

	// Tujuan: nyusun paket data shape supaya siap dipakai di scene atau palet.
	// Input: type nentuin bentuk, position buat anchor awal, filled nyalain isi poligon kalau perlu.
	// Output: ngembaliin ShapeData lengkap berisi outline, isi, warna, dan pivot.
	public ShapeData CreateShape(ShapeType type, Vector2 position, bool filled = true) {
		var outline = BuildOutline(type, position);
		var vertices = filled ? BuildPolygonVertices(type, position) : Array.Empty<Vector2>();
		var color = GetColor(type);
		var pivot = ComputePivot(vertices.Length > 0 ? vertices : outline);

		return new ShapeData {
			OutlinePoints = outline,
			FillVertices = vertices,
			Color = color,
			Pivot = pivot
		};
	}

	// Tujuan: bikin outline bentuk sesuai tipe biar gampang digambar ulang.
	// Input: type berisi enumerasi bentuk dan position jadi titik kiri atas yang dijadikan referensi.
	// Output: ngembaliin daftar titik outline berurutan berbentuk poligon.
	private List<Vector2> BuildOutline(ShapeType type, Vector2 position) {
		float size = UnitSize;
		switch (type) {
			case ShapeType.Persegi:
				return _bentukDasar.Persegi(position.X, position.Y, size);
			case ShapeType.Segitiga:
				return _bentukDasar.SegitigaSamaSisi(position, size);
			case ShapeType.Trapesium: {
				float height = Mathf.Sqrt(size * size - (size * 0.5f) * (size * 0.5f));
				return _bentukDasar.TrapesiumSamaKaki(position, size, size * 2f, height);
			}
			case ShapeType.JajarGenjang: {
				float height = Mathf.Sqrt(size * size - (size * 0.5f) * (size * 0.5f));
				return _bentukDasar.JajarGenjang(position, size, height, -size * 0.5f);
			}
			case ShapeType.JajarGenjangFlip: {
				float height = Mathf.Sqrt(size * size - (size * 0.5f) * (size * 0.5f));
				return _bentukDasar.JajarGenjang(position, size, height, size * 0.5f);
			}
			case ShapeType.JajarGenjang2:
				return _bentukDasar.Rhombus(position, size);
			case ShapeType.JajarGenjang2Flip:
				return _bentukDasar.Rhombus(position, size, true);
			case ShapeType.Hexagon:
				return _bentukDasar.HexagonSamaSisi(position, size);
			default:
				return new List<Vector2>();
		}
	}

	// Tujuan: susun titik isi poligon biar siap buat fill area saat digambar.
	// Input: type mendeskripsikan bentuk dan position dijadikan titik referensi.
	// Output: ngembaliin array Vector2 yang urutannya bisa langsung digambar Godot.
	private Vector2[] BuildPolygonVertices(ShapeType type, Vector2 position) {
		float size = UnitSize;
		switch (type) {
			case ShapeType.Persegi: {
				Vector2 topLeft = position;
				Vector2 topRight = position + new Vector2(size, 0f);
				Vector2 bottomRight = position + new Vector2(size, size);
				Vector2 bottomLeft = position + new Vector2(0f, size);
				return new[] { topLeft, topRight, bottomRight, bottomLeft };
			}
			case ShapeType.Segitiga: {
				float height = Mathf.Sqrt(3f) / 2f * size;
				Vector2 bottomLeft = new Vector2(position.X, position.Y + height);
				Vector2 bottomRight = new Vector2(position.X + size, position.Y + height);
				Vector2 top = new Vector2(position.X + size / 2f, position.Y);
				return new[] { bottomLeft, bottomRight, top };
			}
			case ShapeType.Trapesium: {
				float topWidth = size;
				float bottomWidth = size * 2f;
				float height = Mathf.Sqrt(size * size - (size * 0.5f) * (size * 0.5f));
				float offset = (bottomWidth - topWidth) / 2f;
				Vector2 topLeft = position;
				Vector2 topRight = position + new Vector2(topWidth, 0f);
				Vector2 bottomRight = position + new Vector2(topWidth + offset, height);
				Vector2 bottomLeft = position + new Vector2(-offset, height);
				return new[] { topLeft, topRight, bottomRight, bottomLeft };
			}
			case ShapeType.JajarGenjang: {
				float height = Mathf.Sqrt(size * size - (size * 0.5f) * (size * 0.5f));
				float offset = -size * 0.5f;
				Vector2 topLeft = position;
				Vector2 topRight = position + new Vector2(size, 0f);
				Vector2 bottomRight = position + new Vector2(size + offset, height);
				Vector2 bottomLeft = position + new Vector2(offset, height);
				return new[] { topLeft, topRight, bottomRight, bottomLeft };
			}
			case ShapeType.JajarGenjangFlip: {
				float height = Mathf.Sqrt(size * size - (size * 0.5f) * (size * 0.5f));
				float offset = size * 0.5f;
				Vector2 topLeft = position;
				Vector2 topRight = position + new Vector2(size, 0f);
				Vector2 bottomRight = position + new Vector2(size + offset, height);
				Vector2 bottomLeft = position + new Vector2(offset, height);
				return new[] { topLeft, topRight, bottomRight, bottomLeft };
			}
			case ShapeType.JajarGenjang2:
				return BuildRhombusVertices(position, size, false);
			case ShapeType.JajarGenjang2Flip:
				return BuildRhombusVertices(position, size, true);
			case ShapeType.Hexagon: {
				float radius = size;
				float height = 2f * radius;
				float width = Mathf.Sqrt(3f) * radius;
				Vector2 center = new Vector2(position.X + width / 2f, position.Y + height / 2f);
				Vector2[] vertices = new Vector2[6];
				for (int i = 0; i < 6; i++) {
					float angleDeg = 60f * i + 30f;
					float angleRad = Mathf.DegToRad(angleDeg);
					vertices[i] = new Vector2(
						center.X + radius * Mathf.Cos(angleRad),
						center.Y + radius * Mathf.Sin(angleRad));
				}

				return vertices;
			}
			default:
				return Array.Empty<Vector2>();
		}
	}

	// Tujuan: bangun koordinat belah ketupat dan versi cerminnya.
	// Input: position sebagai titik awal, size buat panjang sisi, flip ngatur posisi mirroring.
	// Output: ngembaliin array titik yang siap dipakai sebagai poligon.
	private Vector2[] BuildRhombusVertices(Vector2 position, float size, bool flip) {
		float angle = Mathf.DegToRad(30f);
		float dx = size * Mathf.Cos(angle);
		float dy = size * Mathf.Sin(angle);

		Vector2 p0 = new Vector2(position.X, position.Y);
		Vector2 p1 = new Vector2(position.X + size, position.Y);
		Vector2 p2 = new Vector2(p1.X + dx, p1.Y + dy);
		Vector2 p3 = new Vector2(p0.X + dx, p0.Y + dy);

		if (!flip) return new[] { p0, p1, p2, p3 };

		float axis = position.X + (size + dx) / 2f;
		Vector2 Mirror(Vector2 point) {
			float reflectedX = axis + (axis - point.X);
			return new Vector2(reflectedX, point.Y);
		}

		Vector2 m0 = Mirror(p0);
		Vector2 m1 = Mirror(p1);
		Vector2 m2 = Mirror(p2);
		Vector2 m3 = Mirror(p3);

		float minX = new[] { m0.X, m1.X, m2.X, m3.X }.Min();
		float shift = position.X - minX;
		return new[] {
			m0 + new Vector2(shift, 0f),
			m1 + new Vector2(shift, 0f),
			m2 + new Vector2(shift, 0f),
			m3 + new Vector2(shift, 0f)
		};
	}

	// Tujuan: milih warna default untuk tiap bentuk supaya mudah dibedakan.
	// Input: type menentukan kategori bentuk.
	// Output: balikin warna standar yang udah disepakati.
	private Color GetColor(ShapeType type) {
		switch (type) {
			case ShapeType.Persegi:
				return Colors.Purple;
			case ShapeType.Segitiga:
				return Colors.Orange;
			case ShapeType.Trapesium:
				return Colors.Yellow;
			case ShapeType.Hexagon:
				return Colors.Red;
			case ShapeType.JajarGenjang:
			case ShapeType.JajarGenjangFlip:
				return Colors.Green;
			case ShapeType.JajarGenjang2:
			case ShapeType.JajarGenjang2Flip:
				return new Color("#1b5e5a");
			default:
				return Colors.White;
		}
	}

	// Tujuan: cari pivot tengah-tengah supaya rotasi tetap stabil.
	// Input: points berisi kumpulan titik outline atau isi poligon.
	// Output: ngembaliin koordinat pivot tengah sebagai Vector2.
	private Vector2 ComputePivot(IEnumerable<Vector2> points) {
		if (points == null) return Vector2.Zero;

		float minX = float.MaxValue, minY = float.MaxValue;
		float maxX = float.MinValue, maxY = float.MinValue;
		bool hasPoint = false;

		foreach (var p in points) {
			hasPoint = true;
			if (p.X < minX) minX = p.X;
			if (p.Y < minY) minY = p.Y;
			if (p.X > maxX) maxX = p.X;
			if (p.Y > maxY) maxY = p.Y;
		}

		if (!hasPoint) return Vector2.Zero;

		return new Vector2((minX + maxX) / 2f, (minY + maxY) / 2f);
	}

	// Tujuan: pastikan resource bentukan dasar dilepas saat objek dibersihkan.
	// Input: what dari lifecycle Godot buat deteksi event yang dipicu engine.
	public override void _Notification(int what) {
		if (what == NotificationPredelete) NodeUtils.DisposeAndNull(_bentukDasar, "_bentukDasar");
	}
}
