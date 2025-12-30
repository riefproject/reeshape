namespace Godot;

using Godot;

public static class ColorUtils {
	// Tujuan: pilih warna palet default berdasarkan index integer simpel.
	// Input: colorIndex angka 1-14 yang dipakai UI.
	// Output: balikin Color siap pakai, default putih kalau gak dikenal.
	public static Color ColorStorage(int colorIndex) {
		switch (colorIndex) {
			case 1:
				return Colors.Red;
			case 2:
				return Colors.Green;
			case 3:
				return Colors.Blue;
			case 4:
				return Colors.Yellow;
			case 5:
				return Colors.Magenta;
			case 6:
				return Colors.Cyan;
			case 7:
				return new Color("#ff7f00");
			case 8:
				return new Color("#ff007f");
			case 9:
				return new Color("#7f00ff");
			case 10:
				return new Color("#007fff");
			case 11:
				return new Color("#00ff7f");
			case 12:
				return new Color("#7fff00");
			case 13:
				return new Color("#a15f10");
			case 14:
				return new Color("#eaa88a");
			default:
				return Colors.White;
		}
	}

	// Tujuan: interpolasi dua warna buat animasi atau efek transisi.
	// Input: a warna awal, b warna akhir, t persentase 0-1.
	// Output: balikin Color hasil campuran linear.
	public static Color LerpColor(Color a, Color b, float t) {
		return a.Lerp(b, t);
	}

	// Tujuan: mencerahkan warna dengan menambah nilai RGB menuju putih.
	// Input: color warna dasar dan amount persentase peningkatan 0-1.
	// Output: warna baru yang lebih terang dengan alfa sama.
	public static Color BrightenColor(Color color, float amount) {
		amount = Mathf.Clamp(amount, 0f, 1f);
		return new Color(
			color.R + (1f - color.R) * amount,
			color.G + (1f - color.G) * amount,
			color.B + (1f - color.B) * amount,
			color.A);
	}

	// Tujuan: menggelapkan warna dengan mengurangi nilai RGB secara proporsional.
	// Input: color warna dasar dan amount proporsi penggelapan 0-1.
	// Output: warna baru yang lebih gelap dengan alfa tetap.
	public static Color DarkenColor(Color color, float amount) {
		amount = Mathf.Clamp(amount, 0f, 1f);
		return new Color(
			color.R * (1f - amount),
			color.G * (1f - amount),
			color.B * (1f - amount),
			color.A);
	}

	// Tujuan: bikin versi warna kebalikan untuk efek kontras tinggi.
	// Input: color warna sumber.
	// Output: warna invers RGB dengan alfa dipertahankan.
	public static Color InvertColor(Color color) {
		return new Color(1f - color.R, 1f - color.G, 1f - color.B, color.A);
	}

	// Tujuan: cari warna komplementer lewat ruang warna HSV.
	// Input: color warna sumber.
	// Output: warna komplementer yang hue-nya digeser 180 derajat.
	public static Color GetComplementaryColor(Color color) {
		color.ToHsv(out float hue, out float saturation, out float value);
		hue = (hue + 0.5f) % 1f;
		return Color.FromHsv(hue, saturation, value, color.A);
	}
}
