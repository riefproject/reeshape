using Godot;
using System.Linq;

public partial class TemplateShapeStage : ShapePlayground {
    // Tujuan: menggambar overlay slot template agar pemain tahu target bentuk.
    private void DrawTemplateOverlay() {
        if (Template == null) return;

        foreach (var slot in _slots) {
            var outline = GetTemplateTransformedPoints(slot.Entry.ShapeType, slot.Entry.Pivot, slot.Entry.AngleRadians, out _);
            var fill = GetTemplateFillVertices(slot.Entry.ShapeType, slot.Entry.Pivot, slot.Entry.AngleRadians, out _);
            if (outline.Count == 0 && fill.Length == 0) continue;

            (Color outlineColor, Color fillColor) = GetSlotColors(slot);
            if (fill.Length > 0) {
                var fillColors = Enumerable.Repeat(fillColor, fill.Length).ToArray();
                DrawPolygon(fill, fillColors);
            }

            if (outline.Count > 0) GraphicsUtils.PutPixelAll(this, outline, GraphicsUtils.DrawStyle.DotDot, outlineColor);
        }
    }

    // Tujuan: menentukan warna outline dan fill berdasarkan status slot.
    // Input: slot target yang sedang digambar.
    // Output: tuple berisi warna outline dan fill sesuai status preview/occupied.
    protected virtual (Color outline, Color fill) GetSlotColors(TemplateSlot slot) => slot.IsPreview ? (PreviewOutlineColor, PreviewFillColor) : (SlotOutlineColor, SlotFillColor);
}
