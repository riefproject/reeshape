namespace Godot;

using Godot;
using System;
using System.Collections.Generic;

public partial class Primitif : RefCounted
{
	public List<Vector2> LineDDA(float xa, float ya, float xb, float yb)
	{
		float dx = xb - xa;
		float dy = yb - ya;
		float steps;
		float xIncrement;
		float yIncrement;
		float x = xa;
		float y = ya;

		List<Vector2> res = new List<Vector2>();

		if (Mathf.Abs(dx) > Mathf.Abs(dy))
		{
			steps = Mathf.Abs(dx);
		}
		else
		{
			steps = Mathf.Abs(dy);
		}

		xIncrement = dx / steps;
		yIncrement = dy / steps;

		res.Add(new Vector2(Mathf.Round(x), Mathf.Round(y)));

		for (int k = 0; k < steps; k++)
		{
			x += xIncrement;
			y += yIncrement;
			res.Add(new Vector2(Mathf.Round(x), Mathf.Round(y)));
		}

		return res;
	}

	public List<Vector2> LineBresenham(float xa, float ya, float xb, float yb)
	{
		List<Vector2> res = new List<Vector2>();
		int x1 = (int)Mathf.Round(xa);
		int y1 = (int)Mathf.Round(ya);
		int x2 = (int)Mathf.Round(xb);
		int y2 = (int)Mathf.Round(yb);

		int dx = Math.Abs(x2 - x1);
		int dy = Math.Abs(y2 - y1);
		int sx = (x1 < x2) ? 1 : -1;
		int sy = (y1 < y2) ? 1 : -1;
		int err = dx - dy;

		while (true)
		{
			res.Add(new Vector2(x1, y1));
			if (x1 == x2 && y1 == y2) break;
			int e2 = 2 * err;
			if (e2 > -dy) { err -= dy; x1 += sx; }
			if (e2 < dx) { err += dx; y1 += sy; }
		}
		return res;
	}

	public List<Vector2> Margin()
	{
		List<Vector2> res = new List<Vector2>();
		res.AddRange(LineBresenham(ScreenUtils.MarginLeft, ScreenUtils.MarginTop, ScreenUtils.MarginRight, ScreenUtils.MarginTop));
		res.AddRange(LineBresenham(ScreenUtils.MarginLeft, ScreenUtils.MarginBottom, ScreenUtils.MarginRight, ScreenUtils.MarginBottom));
		res.AddRange(LineBresenham(ScreenUtils.MarginLeft, ScreenUtils.MarginTop, ScreenUtils.MarginLeft, ScreenUtils.MarginBottom));
		res.AddRange(LineBresenham(ScreenUtils.MarginRight, ScreenUtils.MarginTop, ScreenUtils.MarginRight, ScreenUtils.MarginBottom));
		return res;
	}

	public List<Vector2> Persegi(float x, float y, float ukuran)
	{
		List<Vector2> res = new List<Vector2>();
		res.AddRange(LineBresenham(x, y, x + ukuran, y));
		res.AddRange(LineBresenham(x + ukuran, y, x + ukuran, y + ukuran));
		res.AddRange(LineBresenham(x + ukuran, y + ukuran, x, y + ukuran));
		res.AddRange(LineBresenham(x, y + ukuran, x, y));
		return res;
	}

	public List<Vector2> PersegiPanjang(float x, float y, float panjang, float lebar)
	{
		List<Vector2> res = new List<Vector2>();
		// Sisi atas
		res.AddRange(LineBresenham(x, y, x + panjang, y));
		// Sisi kanan
		res.AddRange(LineBresenham(x + panjang, y, x + panjang, y + lebar));
		// Sisi bawah
		res.AddRange(LineBresenham(x + panjang, y + lebar, x, y + lebar));
		// Sisi kiri
		res.AddRange(LineBresenham(x, y + lebar, x, y));
		return res;
	}

	public List<Vector2> SegitigaSiku(Vector2 titikAwal, int alas, int tinggi)
	{
		List<Vector2> res = new List<Vector2>();
		float x = titikAwal.X;
		float y = titikAwal.Y;
		// Sisi alas
		res.AddRange(LineBresenham(x, y, x + alas, y));
		// Sisi tinggi
		res.AddRange(LineBresenham(x, y, x, y + tinggi));
		// Sisi miring (hipotenusa)
		res.AddRange(LineBresenham(x + alas, y, x, y + tinggi));
		return res;
	}

	public List<Vector2> Segitiga(Vector2 p1, Vector2 p2, Vector2 p3)
	{
		List<Vector2> res = new List<Vector2>();
		res.AddRange(LineBresenham(p1.X, p1.Y, p2.X, p2.Y));
		res.AddRange(LineBresenham(p2.X, p2.Y, p3.X, p3.Y));
		res.AddRange(LineBresenham(p3.X, p3.Y, p1.X, p1.Y));
		return res;
	}

	public List<Vector2> TrapesiumSiku(Vector2 titikAwal, float panjangAtas, float panjangBawah, float tinggi)
	{
		List<Vector2> res = new List<Vector2>();
		float x = titikAwal.X;
		float y = titikAwal.Y;
		// Sisi atas
		res.AddRange(LineBresenham(x, y, x + panjangAtas, y));
		// Sisi miring
		res.AddRange(LineBresenham(x + panjangAtas, y, x + panjangBawah, y + tinggi));
		// Sisi bawah
		res.AddRange(LineBresenham(x + panjangBawah, y + tinggi, x, y + tinggi));
		// Sisi tegak
		res.AddRange(LineBresenham(x, y + tinggi, x, y));
		return res;
	}

	public List<Vector2> TrapesiumSamaKaki(Vector2 titikAwal, float panjangAtas, float panjangBawah, float tinggi)
	{
		List<Vector2> res = new List<Vector2>();
		float x = titikAwal.X;
		float y = titikAwal.Y;
		float offset = (panjangBawah - panjangAtas) / 2.0f;
		
		Vector2 topLeft = new Vector2(x, y);
		Vector2 topRight = new Vector2(x + panjangAtas, y);
		Vector2 bottomLeft = new Vector2(x - offset, y + tinggi);
		Vector2 bottomRight = new Vector2(x + panjangAtas + offset, y + tinggi);

		// Gambar semua sisi
		res.AddRange(LineBresenham(topLeft.X, topLeft.Y, topRight.X, topRight.Y));
		res.AddRange(LineBresenham(topRight.X, topRight.Y, bottomRight.X, bottomRight.Y));
		res.AddRange(LineBresenham(bottomRight.X, bottomRight.Y, bottomLeft.X, bottomLeft.Y));
		res.AddRange(LineBresenham(bottomLeft.X, bottomLeft.Y, topLeft.X, topLeft.Y));
		return res;
	}

	public List<Vector2> JajarGenjang(Vector2 titikAwal, float alas, float tinggi, float jarakBeda)
	{
		List<Vector2> res = new List<Vector2>();
		float x = titikAwal.X;
		float y = titikAwal.Y;

		Vector2 topLeft = new Vector2(x, y);
		Vector2 topRight = new Vector2(x + alas, y);
		Vector2 bottomLeft = new Vector2(x + jarakBeda, y + tinggi);
		Vector2 bottomRight = new Vector2(x + alas + jarakBeda, y + tinggi);

		// Gambar semua sisi
		res.AddRange(LineBresenham(topLeft.X, topLeft.Y, topRight.X, topRight.Y)); // Atas
		res.AddRange(LineBresenham(topRight.X, topRight.Y, bottomRight.X, bottomRight.Y)); // Kanan
		res.AddRange(LineBresenham(bottomRight.X, bottomRight.Y, bottomLeft.X, bottomLeft.Y)); // Bawah
		res.AddRange(LineBresenham(bottomLeft.X, bottomLeft.Y, topLeft.X, topLeft.Y)); // Kiri
		return res;
	}

	public List<Vector2> SegitigaSamaSisi(Vector2 titikAwal, float sisi)
	{
		float tinggi = (float)(Math.Sqrt(3) / 2.0 * sisi);
		Vector2 p1 = new Vector2(titikAwal.X, titikAwal.Y + tinggi); // bottom-left
		Vector2 p2 = new Vector2(titikAwal.X + sisi, titikAwal.Y + tinggi); // bottom-right
		Vector2 p3 = new Vector2(titikAwal.X + sisi / 2, titikAwal.Y); // top-center

		return Segitiga(p1, p2, p3);
	}

	public List<Vector2> HexagonSamaSisi(Vector2 titikAwal, float sisi)
	{
		float radius = sisi;
		// pointy-top hexagon dimensions
		float height = 2 * radius;
		float width = (float)Math.Sqrt(3) * radius;

		// Assuming titikAwal is the top-left of the bounding box
		Vector2 titikPusat = new Vector2(titikAwal.X + width / 2.0f, titikAwal.Y + height / 2.0f);

		List<Vector2> points = new List<Vector2>();
		for (int i = 0; i < 6; i++)
		{
			float angle_deg = 60 * i + 30; // Pointy-top orientation
			float angle_rad = Mathf.DegToRad(angle_deg);
			points.Add(new Vector2(
				titikPusat.X + radius * Mathf.Cos(angle_rad),
				titikPusat.Y + radius * Mathf.Sin(angle_rad)
			));
		}

		List<Vector2> res = new List<Vector2>();
		for (int i = 0; i < points.Count; i++)
		{
			Vector2 p1 = points[i];
			Vector2 p2 = points[(i + 1) % points.Count];
			res.AddRange(LineBresenham(p1.X, p1.Y, p2.X, p2.Y));
		}
		return res;
	}

	public List<Vector2> Rhombus(Vector2 titikAwal, float sisi, bool flip = false)
	{
		List<Vector2> outline = new List<Vector2>();
		float angle = Mathf.DegToRad(30f);
		float dx = sisi * Mathf.Cos(angle);
		float dy = sisi * Mathf.Sin(angle);

		Vector2 p0 = new Vector2(titikAwal.X, titikAwal.Y);
		Vector2 p1 = new Vector2(titikAwal.X + sisi, titikAwal.Y);
		Vector2 p2 = new Vector2(p1.X + dx, p1.Y + dy);
		Vector2 p3 = new Vector2(p0.X + dx, p0.Y + dy);

		if (flip)
		{
			float axis = titikAwal.X + (sisi + dx) / 2f;

			Vector2[] mirrored = new[] { p0, p1, p2, p3 };
			for (int i = 0; i < mirrored.Length; i++)
			{
				float reflectedX = axis + (axis - mirrored[i].X);
				mirrored[i] = new Vector2(reflectedX, mirrored[i].Y);
			}

			float minX = Mathf.Min(Mathf.Min(mirrored[0].X, mirrored[1].X), Mathf.Min(mirrored[2].X, mirrored[3].X));
			float shiftX = titikAwal.X - minX;
			p0 = mirrored[0] + new Vector2(shiftX, 0f);
			p1 = mirrored[1] + new Vector2(shiftX, 0f);
			p2 = mirrored[2] + new Vector2(shiftX, 0f);
			p3 = mirrored[3] + new Vector2(shiftX, 0f);
		}

		outline.AddRange(LineBresenham(p0.X, p0.Y, p1.X, p1.Y));
		outline.AddRange(LineBresenham(p1.X, p1.Y, p2.X, p2.Y));
		outline.AddRange(LineBresenham(p2.X, p2.Y, p3.X, p3.Y));
		outline.AddRange(LineBresenham(p3.X, p3.Y, p0.X, p0.Y));
		return outline;
	}

	public List<Vector2> CircleMidPoint(int xCenter, int yCenter, int radius)
	{
		var points = new List<Vector2>();
		if (radius < 0) return points;

		int x = 0;
		int y = radius;
		int p = 1 - radius; // p0

		CirclePlotPoints(xCenter, yCenter, x, y, points);

		while (x <= y)
		{
			x++;
			if (p < 0)
			{
				// pilih E
				p += 2 * x + 1;
			}
			else
			{
				// pilih SE
				y--;
				p += 2 * (x - y) + 1;
			}
			CirclePlotPoints(xCenter, yCenter, x, y, points);
		}
		return points;
	}

	private void CirclePlotPoints(int xCenter, int yCenter, int x, int y, List<Vector2> points)
	{
		// 8-way symmetry
		points.Add(new Vector2(xCenter + x, yCenter + y));
		points.Add(new Vector2(xCenter - x, yCenter + y));
		points.Add(new Vector2(xCenter + x, yCenter - y));
		points.Add(new Vector2(xCenter - x, yCenter - y));

		points.Add(new Vector2(xCenter + y, yCenter + x));
		points.Add(new Vector2(xCenter - y, yCenter + x));
		points.Add(new Vector2(xCenter + y, yCenter - x));
		points.Add(new Vector2(xCenter - y, yCenter - x));
	}

	public List<Vector2> EllipseMidpoint(int xCenter, int yCenter, int rx, int ry)
	{
		var points = new List<Vector2>();
		if (rx < 0 || ry < 0) return points;

		long rx2 = (long)rx * rx;
		long ry2 = (long)ry * ry;
		long twoRx2 = 2 * rx2;
		long twoRy2 = 2 * ry2;

		int x = 0;
		int y = ry;

		long px = 0;              // 2*ry^2*x
		long py = twoRx2 * y;     // 2*rx^2*y

		// Region 1
		long p1 = ry2 - rx2 * ry + rx2 / 4; // p10 = ry^2 - rx^2*ry + 0.25*rx^2 (dibulatkan)
		EllipsePlotPoints(xCenter, yCenter, x, y, points);

		while (px < py)
		{
			x++;
			px += twoRy2;
			if (p1 < 0)
			{
				p1 += ry2 + px;
			}
			else
			{
				y--;
				py -= twoRx2;
				p1 += ry2 + px - py;
			}
			EllipsePlotPoints(xCenter, yCenter, x, y, points);
		}

		// Region 2
		// p20 = ry^2*(x+0.5)^2 + rx^2*(y-1)^2 - rx^2*ry^2
		// hitung sekali dengan double lalu cast ke long agar tetap integer-increment selanjutnya
		long p2 = (long)Math.Round(ry2 * Math.Pow(x + 0.5, 2) + rx2 * Math.Pow(y - 1, 2) - rx2 * ry2);

		while (y > 0)
		{
			y--;
			py -= twoRx2;
			if (p2 > 0)
			{
				p2 += rx2 - py;
			}
			else
			{
				x++;
				px += twoRy2;
				p2 += rx2 - py + px;
			}
			EllipsePlotPoints(xCenter, yCenter, x, y, points);
		}

		return points;
	}

	private void EllipsePlotPoints(int xCenter, int yCenter, int x, int y, List<Vector2> points)
	{
		// 4-way symmetry
		points.Add(new Vector2(xCenter + x, yCenter + y));
		points.Add(new Vector2(xCenter - x, yCenter + y));
		points.Add(new Vector2(xCenter + x, yCenter - y));
		points.Add(new Vector2(xCenter - x, yCenter - y));
	}
	
	
	// Garis linear "y = m*x + c" di KOORDINAT WORLD, dibatasi xMin..xMax (world)
	public List<Vector2> LineLinearWorld(float m, float c, float xMin, float xMax)
	{
		Vector2 a = new Vector2(xMin, m * xMin + c);
		Vector2 b = new Vector2(xMax, m * xMax + c);
		return LineBresenham(a.X, a.Y, b.X, b.Y);
	}

	// Alternatif: dari 2 titik world (lebih intuitif untuk tata letak)
	public List<Vector2> LineFromPoints(Vector2 a, Vector2 b)
	{
		return LineBresenham(a.X, a.Y, b.X, b.Y);
	}

}
