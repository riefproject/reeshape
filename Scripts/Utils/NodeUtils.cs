namespace Godot;

using Godot;
using System.Collections.Generic;

public static class NodeUtils {
	// Tujuan: bantu dispose sekaligus nul-kan referensi supaya GC gak pegang resource lama.
	// Input: obj adalah instance RefCounted yang mau dibersihkan, objName cuma label buat log.
	public static void DisposeAndNull(RefCounted obj, string objName) {
		GD.Print($"{objName} is null: {obj == null}");
		obj?.Dispose();
		obj = null;
		GD.Print($"{objName} is null after dispose and null: {obj == null}");
	}

	// Tujuan: cek apakah node primitif sudah terisi dan kasih warning kalau belum.
	// Input: primitif adalah referensi node yang wajib ada.
	// Output: balikin list kosong kalau belum siap atau null kalau aman.
	public static List<Vector2> CheckPrimitif(Primitif primitif) {
		if (primitif == null) {
			GD.PrintErr("Node Primitif belum di-assign!");
			return new List<Vector2>();
		}

		return null;
	}
}
