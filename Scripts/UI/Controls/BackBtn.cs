namespace Godot;

using Godot;
using System;

public partial class BackBtn : Button {
    // Tujuan: handler default tombol back yang membawa user ke halaman welcome.
    private void _on_BtnBack_pressed() {
        // Check jika parent adalah DynamicPatternStage, gunakan ReturnScene
        var parent = GetParent();
        while (parent != null) {
            if (parent is DynamicPatternStage) {
                string returnScene = DynamicPatternData.ActiveReturnScene;
                if (string.IsNullOrEmpty(returnScene)) {
                    returnScene = "res://Scenes/Pages/Welcome.tscn";
                }
                GetTree().ChangeSceneToFile(returnScene);
                return;
            }
            parent = parent.GetParent();
        }
        
        // Default: kembali ke Welcome
        GetTree().ChangeSceneToFile("res://Scenes/Pages/Welcome.tscn");
    }
}
