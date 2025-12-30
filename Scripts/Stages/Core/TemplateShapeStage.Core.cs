using Godot;
using System;

public partial class TemplateShapeStage : ShapePlayground {
    // Tujuan: menyiapkan stage template saat node siap digunakan.
    public override void _Ready() {
        base._Ready();
        ResolveHudReferences();
        _snapDistanceBase = SnapDistancePixels;
        ResolveStageIdentity();
        StageSaveService.EnsureLoaded();
        _lastRecord = StageSaveService.GetRecord(_resolvedStageId);
        UpdateBestTimeLabel(_lastRecord?.BestTimeSeconds);
        ResetStageSessionState();
        LoadTemplate();
        UpdateStatusLabel();
        UpdateTimerLabel();
        ArrangeHudLayout();
    }

    // Tujuan: perbarui timer dan label tiap frame jika stage masih berjalan.
    // Input: delta mencatat durasi frame yang diberikan Godot.
    public override void _Process(double delta) {
        base._Process(delta);
        if (_timerRunning) {
            _elapsedSeconds += delta;
            UpdateTimerLabel();
        }
    }

    // Tujuan: menggambar overlay template di atas workspace.
    public override void _Draw() {
        base._Draw();
        DrawTemplateOverlay();
    }

    // Tujuan: callback ketika template berhasil dimuat (dapat dioverride turunan).
    protected virtual void OnTemplateLoaded() { }

    // Tujuan: atur ulang HUD ketika viewport berubah ukuran.
    protected override void OnViewportResized() {
        base.OnViewportResized();
        ArrangeHudLayout();
    }

    // Tujuan: respon saat shape baru ditaruh dari palet.
    // Input: instance adalah ShapeInstance yang baru dibuat.
    protected override void OnShapeSpawned(ShapeInstance instance) {
        base.OnShapeSpawned(instance);
        if (instance == null) return;

        instance.IsLocked = false;
        TrySnapToTemplate(instance, false);
        UpdateStatusLabel();
    }

    // Tujuan: update status ketika shape dipindahkan atau diputar.
    // Input: instance adalah shape yang diubah dan isFinal nandain perubahan permanen.
    protected override void OnShapeTransformUpdated(ShapeInstance instance, bool isFinal) {
        base.OnShapeTransformUpdated(instance, isFinal);
        if (Template == null || instance == null) return;

        bool snapped = TrySnapToTemplate(instance, isFinal);
        if (snapped || isFinal) {
            UpdateStatusLabel();
            QueueRedraw();
        }
    }

    // Tujuan: bersihkan slot ketika shape dihapus dari stage.
    // Input: instance adalah ShapeInstance yang baru dibuang dari workspace.
    protected override void OnShapeRemoved(ShapeInstance instance) {
        base.OnShapeRemoved(instance);
        if (instance == null) return;

        ReleaseSlot(instance);
        UpdateStatusLabel();
        QueueRedraw();
    }
}
