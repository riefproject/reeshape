using Godot;

public partial class ShapePlayground : Node2D {
    public enum StagePreset {
        Default,
        Record
    }

    // Tujuan: inisialisasi layout awal dan load stage sesuai preset saat scene siap.
    public override void _Ready() {
        var background = GetNodeOrNull<Node>("/root/BG");
        background?.Call("slide_to_anchor_y", 0.0);

        var viewport = GetViewport();
        if (viewport != null) {
            ScreenUtils.Initialize(viewport);
            _lastViewportSize = viewport.GetVisibleRect().Size;
        } else {
            _lastViewportSize = Vector2.Zero;
        }

        UpdateLayout();
        OnViewportResized();

        var stage = SelectedStage == StagePreset.Record ? ShapeStageLibrary.RecordStage() : ShapeStageLibrary.DefaultStage();
        ApplyStage(stage);
    }

    // Tujuan: pantau perubahan ukuran viewport dan refresh layout jika perlu.
    // Input: delta menyatakan durasi frame dari loop Godot.
    public override void _Process(double delta) {
        base._Process(delta);

        var viewport = GetViewport();
        if (viewport == null) return;

        Vector2 currentSize = viewport.GetVisibleRect().Size;
        if (!currentSize.IsEqualApprox(_lastViewportSize)) {
            _lastViewportSize = currentSize;
            ScreenUtils.Initialize(viewport);
            UpdateLayout();
            OnViewportResized();
            HidePaletteTooltip();
            QueueRedraw();
        }
    }

    // Tujuan: hitung ulang posisi palet kanan sesuai ukuran layar sekarang.
    // Input: menggunakan data ScreenUtils yang sudah diinialisasi sebelumnya.
    private void UpdateLayout() {
        float vw = ScreenUtils.ScreenWidth;
        float vh = ScreenUtils.ScreenHeight;
        float margin = Mathf.Max(32f, vh * 0.045f);
        float bottomMargin = Mathf.Max(90f, vh * 0.125f);
        float libraryWidth = Mathf.Max(200f, vw * 0.156f);

        _libraryRect = new Rect2(
            vw - libraryWidth - margin,
            margin,
            libraryWidth,
            vh - margin - bottomMargin
        );
    }

    // Tujuan: terapkan konfigurasi stage dan susun ulang palet shape.
    // Input: stage berisi definisi stage yang akan dimainkan.
    public void ApplyStage(ShapeStageDefinition stage) {
        if (stage == null) {
            GD.PrintErr("[ShapePlayground] Stage definition is null.");
            return;
        }

        _shapeCache.Clear();
        _palette.Clear();
        _activeShapes.Clear();
        _shapeLabels.Clear();
        SetGlobalShapeScale(1f);

        _unitSize = stage.UnitSize;
        _paletteSpacing = stage.PaletteSpacing;
        _paletteStart = stage.PaletteStart;
        _paletteTileSize = stage.PaletteTileSize;
        _palettePreviewScale = stage.PalettePreviewScale;
        _rotationStep = Mathf.DegToRad(stage.RotationStepDegrees);
        _shapeLibrary.UnitSize = _unitSize;

        for (int i = 0; i < stage.Quotas.Count; i++) {
            var quota = stage.Quotas[i];
            _shapeLabels[quota.Type] = quota.DisplayName;
            var prebuilt = GetShapeDefinition(quota.Type);

            _palette.Add(new PaletteEntry {
                Quota = quota,
                Remaining = quota.IsUnlimited ? int.MaxValue : quota.MaxCopies,
                IsUnlimited = quota.IsUnlimited,
                PreviewPosition = Vector2.Zero,
                Shape = prebuilt
            });
        }

        _selectedShape = null;
        _draggedShape = null;
        _dragOffset = Vector2.Zero;
        QueueRedraw();
    }

    // Tujuan: sediakan area kerja utama yang bebas dari panel samping.
    // Input: memakai dimensi layar dari ScreenUtils.
    // Output: Rect2 yang mendefinisikan workspace saat ini.
    protected Rect2 GetWorkspaceRect() {
        float vw = ScreenUtils.ScreenWidth;
        float vh = ScreenUtils.ScreenHeight;
        float margin = Mathf.Max(32f, vh * 0.045f);
        float bottomMargin = Mathf.Max(90f, vh * 0.125f);
        float libraryWidth = Mathf.Max(200f, vw * 0.156f);
        float gap = Mathf.Max(20f, vw * 0.0156f);

        return new Rect2(
            new Vector2(margin, margin),
            new Vector2(
                vw - margin * 2f - libraryWidth - gap,
                vh - margin - bottomMargin)
        );
    }

    // Tujuan: hook kosong agar turunan dapat menambah perilaku saat viewport berubah.
    protected virtual void OnViewportResized() { }

    // Tujuan: membersihkan resource ketika node dikeluarkan dari scene tree.
    public override void _ExitTree() {
        HidePaletteTooltip();
        NodeUtils.DisposeAndNull(_shapeLibrary, nameof(_shapeLibrary));
        base._ExitTree();
    }
}
