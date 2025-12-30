using Godot;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Matrix4x4 = System.Numerics.Matrix4x4;
using ShapeType = PatternShapeLibrary.ShapeType;

public partial class ShapePlayground {
    // Tujuan: delegasi input Godot ke handler mouse atau keyboard.
    // Input: @event berisi event input yang diterima frame ini.
    public override void _Input(InputEvent @event) {
        if (@event is InputEventMouseButton mouseButton) {
            HandleMouseButton(mouseButton);
            return;
        }

        if (@event is InputEventMouseMotion motion) {
            if (_isPanning) {
                _viewOffset += motion.Relative;
                QueueRedraw();
            } else if (_draggedShape != null) {
                var newPivot = ScreenToWorld(motion.Position) - _dragOffset;
                if (SnapToPixelGrid) newPivot = newPivot.Round();
                _draggedShape.WorldPivot = newPivot;
                OnShapeTransformUpdated(_draggedShape, false);
                QueueRedraw();
            }

            UpdatePaletteTooltip(motion.Position);
            return;
        }

        if (@event is InputEventKey keyEvent && keyEvent.Pressed && !keyEvent.Echo) HandleKeyInput(keyEvent);
    }

    // Tujuan: memproses klik mouse untuk zoom, pan, spawn, memilih, dan menghapus shape.
    // Input: mouseButton berisi informasi tombol dan posisi layar.
    private void HandleMouseButton(InputEventMouseButton mouseButton) {
        var worldPos = ScreenToWorld(mouseButton.Position);

        if (mouseButton.ButtonIndex == MouseButton.WheelUp || mouseButton.ButtonIndex == MouseButton.WheelDown) {
            if (!mouseButton.Pressed) return;

            float zoomFactor = 1.1f;
            _viewScale = mouseButton.ButtonIndex == MouseButton.WheelUp ? _viewScale * zoomFactor : _viewScale / zoomFactor;
            _viewOffset = mouseButton.Position - worldPos * _viewScale;
            QueueRedraw();
            return;
        }

        if (mouseButton.ButtonIndex == MouseButton.Middle) {
            _isPanning = mouseButton.Pressed;
            return;
        }

        if (mouseButton.ButtonIndex == MouseButton.Left) {
            if (mouseButton.Pressed) {
                if (TryPickShape(worldPos)) {
                    QueueRedraw();
                    return;
                }

                if (TrySpawnFromPalette(mouseButton.Position, worldPos)) {
                    QueueRedraw();
                    return;
                }
            } else {
                if (_draggedShape != null) OnShapeTransformUpdated(_draggedShape, true);
                _draggedShape = null;
                _dragOffset = Vector2.Zero;
            }

            UpdatePaletteTooltip(mouseButton.Position);
            return;
        }

        if (mouseButton.ButtonIndex == MouseButton.Right && mouseButton.Pressed) {
            var shape = FindShapeUnderPosition(worldPos);
            if (shape != null) {
                RemoveShape(shape, true);
                QueueRedraw();
            }
        }

        UpdatePaletteTooltip(mouseButton.Position);
    }

    // Tujuan: menangani kombinasi tombol keyboard untuk menggeser, memutar, dan mengekspor stage.
    // Input: keyEvent adalah event keyboard yang sudah diverifikasi ditekan.
    private protected virtual void HandleKeyInput(InputEventKey keyEvent) {
        if (_selectedShape == null) return;

        switch (keyEvent.Keycode) {
            case Key.W:
            case Key.Up:
                MoveSelected(new Vector2(0f, -GetMovementStep(keyEvent)), SelectedStage == StagePreset.Record);
                break;
            case Key.S:
            case Key.Down:
                MoveSelected(new Vector2(0f, GetMovementStep(keyEvent)), SelectedStage == StagePreset.Record);
                break;
            case Key.A:
            case Key.Left:
                MoveSelected(new Vector2(-GetMovementStep(keyEvent), 0f), SelectedStage == StagePreset.Record);
                break;
            case Key.D:
            case Key.Right:
                MoveSelected(new Vector2(GetMovementStep(keyEvent), 0f), SelectedStage == StagePreset.Record);
                break;
            case Key.Q:
                RotateSelected(-_rotationStep);
                break;
            case Key.E:
                RotateSelected(_rotationStep);
                break;
            case Key.Delete:
            case Key.Backspace:
                RemoveShape(_selectedShape, true);
                QueueRedraw();
                break;
            case Key.P:
                ExportLayout();
                break;
        }
    }

    // Tujuan: memutar shape terpilih dengan kelipatan step yang sudah ditentukan.
    // Input: delta yaitu radian rotasi yang ingin diterapkan.
    private void RotateSelected(float delta) {
        if (_selectedShape == null || _selectedShape.IsLocked) return;

        float stepSize = _rotationStep;
        if (Mathf.IsZeroApprox(stepSize)) return;

        float target = _selectedShape.Rotation + delta;
        float stepIndex = Mathf.Round(target / stepSize);
        _selectedShape.Rotation = Mathf.Wrap(stepIndex * stepSize, -Mathf.Pi, Mathf.Pi);
        OnShapeTransformUpdated(_selectedShape, SelectedStage == StagePreset.Record);
        QueueRedraw();
    }

    // Tujuan: memberikan jarak perpindahan per satu aksi keyboard.
    // Input: keyEvent pemicu gerakan (belum dimanfaatkan untuk variasi).
    // Output: float jarak perpindahan (saat ini selalu 1 piksel).
    private float GetMovementStep(InputEventKey keyEvent) => 1f;

    // Tujuan: menggeser shape terpilih sesuai delta dan menerapkan snap bila diminta.
    // Input: delta (Vector2) perpindahan dan applySnap untuk menentukan perlu snap.
    private void MoveSelected(Vector2 delta, bool applySnap) {
        if (_selectedShape == null || _selectedShape.IsLocked) return;
        _selectedShape.WorldPivot += delta;
        if (SnapToPixelGrid) _selectedShape.WorldPivot = _selectedShape.WorldPivot.Round();
        if (applySnap) SnapSelectedToNeighbors();
        OnShapeTransformUpdated(_selectedShape, applySnap);
        QueueRedraw();
    }

    // Tujuan: merapatkan shape terpilih ke tetangga terdekat saat mode rekaman aktif.
    private void SnapSelectedToNeighbors() {
        if (SelectedStage != StagePreset.Record) return;
        if (_selectedShape == null || _selectedShape.IsLocked) return;
        if (_activeShapes.Count <= 1) return;

        float snapThreshold = Mathf.Max(10f * _globalShapeScale, _unitSize * _globalShapeScale * 0.25f);

        GetBounds(_selectedShape, out float selMinX, out float selMaxX, out float selMinY, out float selMaxY);
        float bestShiftX = 0f;
        float bestDistX = snapThreshold + 1f;

        foreach (var neighbor in _activeShapes) {
            if (neighbor == _selectedShape) continue;
            GetBounds(neighbor, out float nbMinX, out float nbMaxX, out float nbMinY, out float nbMaxY);

            float diff = nbMaxX - selMinX;
            float abs = Mathf.Abs(diff);
            if (abs < bestDistX && abs <= snapThreshold) {
                bestDistX = abs;
                bestShiftX = diff;
            }

            diff = nbMinX - selMaxX;
            abs = Mathf.Abs(diff);
            if (abs < bestDistX && abs <= snapThreshold) {
                bestDistX = abs;
                bestShiftX = diff;
            }
        }

        if (!Mathf.IsZeroApprox(bestShiftX)) _selectedShape.WorldPivot += new Vector2(bestShiftX, 0f);

        GetBounds(_selectedShape, out selMinX, out selMaxX, out selMinY, out selMaxY);
        float bestShiftY = 0f;
        float bestDistY = snapThreshold + 1f;

        foreach (var neighbor in _activeShapes) {
            if (neighbor == _selectedShape) continue;
            GetBounds(neighbor, out float nbMinX, out float nbMaxX, out float nbMinY, out float nbMaxY);

            float diff = nbMaxY - selMinY;
            float abs = Mathf.Abs(diff);
            if (abs < bestDistY && abs <= snapThreshold) {
                bestDistY = abs;
                bestShiftY = diff;
            }

            diff = nbMinY - selMaxY;
            abs = Mathf.Abs(diff);
            if (abs < bestDistY && abs <= snapThreshold) {
                bestDistY = abs;
                bestShiftY = diff;
            }
        }

        if (!Mathf.IsZeroApprox(bestShiftY)) _selectedShape.WorldPivot += new Vector2(0f, bestShiftY);
    }

    // Tujuan: mendapatkan bounding box shape setelah transformasi.
    // Input: shape instance, lalu hasil minimum/maksimum dikeluarkan lewat parameter out.
    private void GetBounds(ShapeInstance shape, out float minX, out float maxX, out float minY, out float maxY) {
        var points = BuildTransformedPoints(shape);
        if (points.Count == 0) {
            minX = maxX = shape.WorldPivot.X;
            minY = maxY = shape.WorldPivot.Y;
            return;
        }

        minX = points.Min(p => p.X);
        maxX = points.Max(p => p.X);
        minY = points.Min(p => p.Y);
        maxY = points.Max(p => p.Y);
    }

    // Tujuan: memilih shape teratas di posisi world tertentu untuk mulai drag.
    // Input: worldPos posisi dunia tempat user mengklik.
    // Output: true bila shape berhasil dipilih, false bila tidak ada.
    private bool TryPickShape(Vector2 worldPos) {
        for (int i = _activeShapes.Count - 1; i >= 0; i--) {
            var shape = _activeShapes[i];
            if (shape.IsLocked) continue;
            if (!IsPointInsideShape(worldPos, shape)) continue;

            _selectedShape = shape;
            _draggedShape = shape;
            _dragOffset = worldPos - shape.WorldPivot;

            _activeShapes.RemoveAt(i);
            _activeShapes.Add(shape);
            return true;
        }
        return false;
    }

    // Tujuan: membuat instance shape baru dari palet ketika slot diklik.
    // Input: screenPos lokasi klik di layar, worldPos lokasi pivot di dunia.
    // Output: true bila shape berhasil dibuat, false bila klik tidak mengenai slot.
    private bool TrySpawnFromPalette(Vector2 screenPos, Vector2 worldPos) {
        for (int i = 0; i < _palette.Count; i++) {
            var entry = _palette[i];
            if (!entry.IsUnlimited && entry.Remaining <= 0) continue;

            var rect = GetPaletteRect(i);
            if (!rect.HasPoint(screenPos)) continue;

            var instance = new ShapeInstance {
                Definition = entry.Shape,
                WorldPivot = worldPos,
                Rotation = 0f,
                PaletteIndex = i,
                ShapeType = entry.Quota.Type
            };

            if (!entry.IsUnlimited) entry.Remaining -= 1;
            _palette[i] = entry;

            _activeShapes.Add(instance);
            _selectedShape = instance;
            _draggedShape = instance;
            _dragOffset = worldPos - instance.WorldPivot;
            OnShapeSpawned(instance);
            OnShapeTransformUpdated(instance, false);
            return true;
        }
        return false;
    }

    // Tujuan: mencari shape terakhir yang berada di posisi world tertentu.
    // Input: worldPos yang dicek.
    // Output: shape yang ditemukan atau null bila tidak ada.
    private ShapeInstance FindShapeUnderPosition(Vector2 worldPos) {
        for (int i = _activeShapes.Count - 1; i >= 0; i--) {
            var shape = _activeShapes[i];
            if (IsPointInsideShape(worldPos, shape)) return shape;
        }
        return null;
    }

    // Tujuan: konversi posisi layar ke posisi dunia berdasarkan zoom dan offset.
    // Input: screenPos koordinat layar dari pointer.
    // Output: Vector2 posisi dunia setelah dikoreksi zoom/offset.
    private Vector2 ScreenToWorld(Vector2 screenPos) => (screenPos - _viewOffset) / _viewScale;

    // Tujuan: memberikan posisi pointer mouse di koordinat dunia.
    // Output: Vector2 posisi pointer saat ini dalam koordinat dunia.
    private Vector2 GetWorldMousePosition() => ScreenToWorld(GetViewport().GetMousePosition());
}
