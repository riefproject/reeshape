using Godot;
using System;
using System.Collections.Generic;

public static class FungsiMatematika {
    // Tujuan: generator umum buat bikin titik berdasarkan fungsi f(x) di kuadran kanan.
    // Input: f fungsi evaluasi, xStart titik awal sampling, step jarak antar sampel.
    // Output: balikin list titik Vector2 yang lolos filter layar.
    private static List<Vector2> Generate(Func<float, float> f, float xStart, float step) {
        var points = new List<Vector2>();
        float xEnd = ScreenUtils.Center.X - ScreenUtils.MarginLeft;
        for (float x = xStart; x < xEnd; x += step) {
            float y = f(x);
            if (y > 0 && y < ScreenUtils.Center.Y - ScreenUtils.MarginTop) points.Add(new Vector2(x, y));
        }

        return points;
    }

    // Tujuan: bikin kurva nilai mutlak sederhana y = |a * x^2 - c| buat materi grafis.
    // Input: a ngatur kelengkungan parabola dan c geser kurva secara vertikal.
    // Output: daftar titik Vector2 yang bisa langsung digambar.
    public static List<Vector2> Mutlak(float a, float c) {
        return Generate(x => Mathf.Abs(a * x * x - c), 0, 0.5f);
    }

    // Tujuan: gambar fungsi eksponensial y = a * e^(x/b) di layar koordinat positif.
    // Input: a sebagai skala vertikal dan b penentu laju kenaikan.
    // Output: list titik yang menggambarkan kurva.
    public static List<Vector2> Eksponensial(float a, float b) {
        return Generate(x => a * Mathf.Exp(x / b), 0, 0.5f);
    }

    // Tujuan: hasilkan garis lurus y = m * x + c buat latihan analisis.
    // Input: m gradient garis dan c potong sumbu y.
    // Output: list titik Vector2 berjarak 1 satuan.
    public static List<Vector2> Linier(float m, float c) {
        return Generate(x => m * x + c, 0, 1f);
    }

    // Tujuan: sediakan titik kurva kuadrat dasar y = a * x^2.
    // Input: a ngatur apakah parabola membuka atas/bawah dan seberapa lebar.
    // Output: list titik Vector2 di kuadran kanan.
    public static List<Vector2> Kuadrat(float a) {
        return Generate(x => a * x * x, 0, 0.5f);
    }

    // Tujuan: generator umum buat fungsi simetris kanan-kiri sekitar titik nol.
    // Input: f fungsi evaluasi dan step jarak antar sampel.
    // Output: list titik Vector2 hasil filter area layar.
    private static List<Vector2> GenerateSymmetric(Func<float, float> f, float step) {
        var points = new List<Vector2>();
        float xEnd = ScreenUtils.Center.X - ScreenUtils.MarginLeft;
        for (float x = -xEnd; x < xEnd; x += step) {
            float y = f(x);
            if (Mathf.Abs(y) < ScreenUtils.Center.Y - ScreenUtils.MarginTop) points.Add(new Vector2(x, y));
        }

        return points;
    }

    // Tujuan: bikin kurva sinus dengan amplitudo dan frekuensi kustom.
    // Input: amplitude ngatur tinggi gelombang dan frequency seberapa rapat gelombangnya.
    // Output: list titik sinus siap digambar.
    public static List<Vector2> Sinus(float amplitude, float frequency) {
        return GenerateSymmetric(x => amplitude * Mathf.Sin(x * frequency), 0.05f);
    }

    // Tujuan: bangun kurva cosinus buat kebutuhan materi trigonometri.
    // Input: amplitude tentuin tinggi gelombang dan frequency kerapatan siklus.
    // Output: list titik cosinus yang sudah ter-sample.
    public static List<Vector2> Cosinus(float amplitude, float frequency) {
        return GenerateSymmetric(x => amplitude * Mathf.Cos(x * frequency), 0.05f);
    }

    // Tujuan: sediakan data kurva tangen dengan parameter fleksibel.
    // Input: amplitude ngatur tinggi puncak dan frequency nentuin jarak antar asimtot.
    // Output: list titik tangen yang sudah dibatasi sesuai viewport.
    public static List<Vector2> Tangen(float amplitude, float frequency) {
        return GenerateSymmetric(x => amplitude * Mathf.Tan(x * frequency), 0.05f);
    }
}
