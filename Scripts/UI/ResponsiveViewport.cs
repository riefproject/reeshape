using Godot;

public partial class ResponsiveViewport : Node {
    private static readonly Vector2I TargetSize = new(1280, 720);

    // Tujuan: pas start langsung kunci viewport, atur ukuran awal, dan siap dengerin resize.
    public override void _Ready() {
        var window = GetWindow();
        if (window == null) return;

        ApplyViewportScaling(window);
        ApplyWindowDefaults(window);
        window.SizeChanged += OnWindowSizeChanged;
    }

    // Tujuan: ngatur konten supaya skalanya tetep proporsional dan kasih warna frame hitam.
    // Input: window instance yang lagi aktif.
    private void ApplyViewportScaling(Window window) {
        window.ContentScaleMode = Window.ContentScaleModeEnum.Viewport;
        window.ContentScaleAspect = Window.ContentScaleAspectEnum.Keep;
        UpdateContentScaleSize(window, window.Size);
        RenderingServer.SetDefaultClearColor(Colors.Black);
    }

    // Tujuan: ngatur window biar begitu launch langsung pakai ukuran baseline 1280x720.
    // Input: window instance aplikasi.
    private void ApplyWindowDefaults(Window window) {
        window.Size = TargetSize;
        UpdateContentScaleSize(window, window.Size);
    }

    // Tujuan: update setiap kali user resize window supaya viewport tetap 16:9 dan gak pecah.
    private void OnWindowSizeChanged() {
        var window = GetWindow();
        if (window == null) return;

        UpdateContentScaleSize(window, window.Size);
    }

    // Tujuan: hitung ulang content scale size yang pas dengan ratio 16:9.
    // Input: window target dan windowSize ukuran frame terbaru.
    private void UpdateContentScaleSize(Window window, Vector2I windowSize) {
        window.ContentScaleSize = ResolveAspectFit(windowSize);
    }

    // Tujuan: cari ukuran paling cocok biar width/height tetep 16:9 tanpa nilai nol.
    // Input: rawSize ukuran window mentah dari sistem.
    // Output: Vector2I hasil penyesuaian rasio 16:9.
    private Vector2I ResolveAspectFit(Vector2I rawSize) {
        if (rawSize.X <= 0 || rawSize.Y <= 0) return TargetSize;

        const float aspect = 16f / 9f;
        float width = rawSize.X;
        float height = rawSize.Y;

        if (width / height > aspect) width = height * aspect;
        else height = width / aspect;

        int finalWidth = Mathf.Max(1, Mathf.RoundToInt(width));
        int finalHeight = Mathf.Max(1, Mathf.RoundToInt(height));
        return new Vector2I(finalWidth, finalHeight);
    }
}
