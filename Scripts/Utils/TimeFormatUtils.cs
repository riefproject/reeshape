using System;

public static class TimeFormatUtils {
    // Tujuan: konversi detik jadi teks mm:ss.xx atau hh:mm:ss.xx kalau lebih dari sejam.
    // Input: seconds berupa angka detik yang mau ditampilkan.
    // Output: balikin string siap tampilan scoreboard.
    public static string FormatElapsedTime(double seconds) {
        if (double.IsNaN(seconds) || double.IsInfinity(seconds) || seconds < 0) return "--:--";

        TimeSpan span = TimeSpan.FromSeconds(seconds);
        if (span.TotalHours >= 1) return string.Format("{0:D2}:{1:D2}:{2:D2}.{3:D2}", (int)span.TotalHours, span.Minutes, span.Seconds, span.Milliseconds / 10);

        return string.Format("{0:D2}:{1:D2}.{2:D2}", span.Minutes, span.Seconds, span.Milliseconds / 10);
    }

    // Tujuan: format waktu dengan fallback placeholder kalau belum ada catatan.
    // Input: seconds nullable detik dan placeholder teks pengganti.
    // Output: balikin string hasil FormatElapsedTime atau placeholder.
    public static string FormatElapsedTime(double? seconds, string placeholder = "--:--") {
        if (!seconds.HasValue) return placeholder;

        return FormatElapsedTime(seconds.Value);
    }
}
