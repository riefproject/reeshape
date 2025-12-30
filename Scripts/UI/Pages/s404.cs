using Godot;
using System;

public partial class s404 : Control {
    // Tujuan: geser background ke posisi default saat halaman 404 muncul.
    public override void _Ready() {
        var background = GetNode<Node>("/root/BG");
        background.Call("slide_to_anchor_y", 0.0);
    }

    // Tujuan: kembali ke halaman welcome ketika tombol back ditekan.
    private void _on_BtnBack_pressed() {
        GetTree().ChangeSceneToFile("res://Scenes/Pages/Welcome.tscn");
    }

    // Tujuan: override kosong untuk konsistensi; tidak ada logika per-frame yang diperlukan.
    // Input: delta berisi selang waktu antar frame.
    public override void _Process(double delta) { }
}
