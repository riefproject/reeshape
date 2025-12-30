namespace Godot;

using Godot;
using System.Collections.Generic;
using System.Numerics;

public static class PrintUtils {
	// Tujuan: bantu ngeprint isi list Vector2 biar gampang dicek di console.
	// Input: vectorList adalah data yang mau dibaca, listName jadi label teks.
	public static void PrintVector2List(List<Vector2> vectorList, string listName = "Vector2 List") {
		if (vectorList == null) {
			GD.Print($"{listName} is null.");
			return;
		}

		GD.Print($"{listName}:");
		foreach (Vector2 vector in vectorList) GD.Print($"  ({vector.X}, {vector.Y})");
	}

	// Tujuan: tampilkan matriks 4x4 dengan format kolom rapi di log Godot.
	// Input: matrix adalah data yang mau dicek, label jadi judul output.
	public static void PrintMatrix(Matrix4x4 matrix, string label = "Matrix") {
		GD.Print($"{label}:");
		GD.Print($"| {matrix.M11,6:F2} {matrix.M12,6:F2} {matrix.M13,6:F2} {matrix.M14,6:F2} |");
		GD.Print($"| {matrix.M21,6:F2} {matrix.M22,6:F2} {matrix.M23,6:F2} {matrix.M24,6:F2} |");
		GD.Print($"| {matrix.M31,6:F2} {matrix.M32,6:F2} {matrix.M33,6:F2} {matrix.M34,6:F2} |");
		GD.Print($"| {matrix.M41,6:F2} {matrix.M42,6:F2} {matrix.M43,6:F2} {matrix.M44,6:F2} |");
	}

	// Tujuan: print Vector3 dalam format sederhana buat debugging transform 3D.
	// Input: vector sebagai nilai yang mau dilihat dan label untuk judulnya.
	public static void PrintVector3(Vector3 vector, string label = "Vector3") {
		GD.Print($"{label}: ({vector.X:F2}, {vector.Y:F2}, {vector.Z:F2})");
	}
}
