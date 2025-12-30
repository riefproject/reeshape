using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public static class TemplateLoader {
	public sealed class TemplateEntry {
		public PatternShapeLibrary.ShapeType ShapeType;
		public Vector2 Pivot;
		public float AngleRadians;
	}

	public sealed class ShapeTemplate {
		public string Name;
		public List<TemplateEntry> Entries = new();
		public float? Scale;
	}

	// JSON structure classes
	private class JsonTemplate {
		public string name { get; set; }
		public float scale { get; set; } = 1.0f;
		public List<JsonShape> shapes { get; set; }
	}

	private class JsonShape {
		public string type { get; set; }
		public float x { get; set; }
		public float y { get; set; }
		public float angle { get; set; }
	}

	// Tujuan: baca file JSON template dan ubah jadi struktur template siap pakai.
	// Input: path adalah lokasi file JSON yang berisi daftar shape.
	// Output: balikin ShapeTemplate atau null kalau parsing gagal.
	public static ShapeTemplate LoadFromReceipt(string path) {
		if (!File.Exists(path)) {
			GD.PrintErr($"[TemplateLoader] File tidak ditemukan: {path}");
			return null;
		}

		string ext = Path.GetExtension(path).ToLowerInvariant();
		if (ext != ".json") {
			GD.PrintErr($"[TemplateLoader] Hanya support format JSON. File: {path}");
			return null;
		}

		return LoadFromJson(path);
	}

	// Tujuan: load template dari format JSON
	private static ShapeTemplate LoadFromJson(string path) {
		try {
			string jsonContent = File.ReadAllText(path);
			var jsonTemplate = JsonSerializer.Deserialize<JsonTemplate>(jsonContent, new JsonSerializerOptions {
				PropertyNameCaseInsensitive = true
			});

			if (jsonTemplate == null || jsonTemplate.shapes == null) {
				GD.PrintErr($"[TemplateLoader] JSON parsing failed: {path}");
				return null;
			}

			var template = new ShapeTemplate {
				Name = jsonTemplate.name ?? Path.GetFileNameWithoutExtension(path),
				Scale = jsonTemplate.scale
			};

			foreach (var shape in jsonTemplate.shapes) {
				if (!TryMapShapeName(shape.type, out var shapeType)) {
					GD.PrintErr($"[TemplateLoader] Bentuk '{shape.type}' tidak dikenali, dilewati.");
					continue;
				}

				template.Entries.Add(new TemplateEntry {
					ShapeType = shapeType,
					Pivot = new Vector2(shape.x, shape.y),
					AngleRadians = Mathf.DegToRad(shape.angle)
				});
			}

			if (template.Entries.Count == 0) {
				GD.PrintErr($"[TemplateLoader] Tidak ada entry valid di {path}");
				return null;
			}

			GD.Print($"[TemplateLoader] Loaded '{template.Name}' dari JSON: {template.Entries.Count} shapes, scale={template.Scale}");
			return template;

		} catch (Exception ex) {
			GD.PrintErr($"[TemplateLoader] Error parsing JSON {path}: {ex.Message}");
			return null;
		}
	}

	// Tujuan: save template ke format JSON
	public static bool SaveToJson(string path, string name, float scale, List<TemplateEntry> entries) {
		try {
			var jsonShapes = new List<JsonShape>();
			foreach (var entry in entries) {
				jsonShapes.Add(new JsonShape {
					type = GetShapeTypeName(entry.ShapeType),
					x = entry.Pivot.X,
					y = entry.Pivot.Y,
					angle = Mathf.RadToDeg(entry.AngleRadians)
				});
			}

			var jsonTemplate = new JsonTemplate {
				name = name,
				scale = scale,
				shapes = jsonShapes
			};

			var options = new JsonSerializerOptions {
				WriteIndented = true,
				PropertyNamingPolicy = JsonNamingPolicy.CamelCase
			};

			string jsonContent = JsonSerializer.Serialize(jsonTemplate, options);
			File.WriteAllText(path, jsonContent);

			GD.Print($"[TemplateLoader] Saved '{name}' to JSON: {entries.Count} shapes");
			return true;

		} catch (Exception ex) {
			GD.PrintErr($"[TemplateLoader] Error saving JSON {path}: {ex.Message}");
			return false;
		}
	}

	// Tujuan: list semua file JSON di folder tertentu
	public static List<string> GetAllTemplateFiles(string folderPath) {
		var files = new List<string>();
		
		if (!Directory.Exists(folderPath)) {
			GD.PrintErr($"[TemplateLoader] Folder tidak ditemukan: {folderPath}");
			return files;
		}

		foreach (var file in Directory.GetFiles(folderPath, "*.json")) {
			files.Add(file);
		}

		return files;
	}

	// Tujuan: ambil scale dari JSON template tanpa load semua entries
	public static float GetTemplateScale(string path) {
		try {
			if (!File.Exists(path)) {
				GD.PrintErr($"[TemplateLoader] File tidak ditemukan: {path}");
				return 1.0f;
			}

			string jsonContent = File.ReadAllText(path);
			var jsonTemplate = JsonSerializer.Deserialize<JsonTemplate>(jsonContent, new JsonSerializerOptions {
				PropertyNameCaseInsensitive = true
			});

			return jsonTemplate?.scale ?? 1.0f;

		} catch (Exception ex) {
			GD.PrintErr($"[TemplateLoader] Error reading scale from {path}: {ex.Message}");
			return 1.0f;
		}
	}

	// Tujuan: convert ShapeType enum ke nama string untuk JSON
	private static string GetShapeTypeName(PatternShapeLibrary.ShapeType type) {
		switch (type) {
			case PatternShapeLibrary.ShapeType.Persegi:
				return "Persegi";
			case PatternShapeLibrary.ShapeType.Segitiga:
				return "Segitiga";
			case PatternShapeLibrary.ShapeType.Trapesium:
				return "Trapesium";
			case PatternShapeLibrary.ShapeType.JajarGenjang:
				return "Jajar Genjang";
			case PatternShapeLibrary.ShapeType.JajarGenjang2:
				return "Belah Ketupat";
			case PatternShapeLibrary.ShapeType.Hexagon:
				return "Hexagon";
			default:
				return "Unknown";
		}
	}

	// Tujuan: mapping nama bentuk bebas dari file ke enum internal library.
	// Input: name adalah label bentuk dari file.
	// Output: balikin true kalau berhasil dan keluarin tipe di parameter out.
	private static bool TryMapShapeName(string name, out PatternShapeLibrary.ShapeType type) {
		switch (name.ToLowerInvariant()) {
			case "persegi":
				type = PatternShapeLibrary.ShapeType.Persegi;
				return true;
			case "segitiga":
				type = PatternShapeLibrary.ShapeType.Segitiga;
				return true;
			case "trapesium":
				type = PatternShapeLibrary.ShapeType.Trapesium;
				return true;
			case "jajar genjang":
			case "jajargenjang":
				type = PatternShapeLibrary.ShapeType.JajarGenjang;
				return true;
			case "jajar genjang2":
			case "jajargenjang2":
			case "belah ketupat":
				type = PatternShapeLibrary.ShapeType.JajarGenjang2;
				return true;
			case "hexagon":
				type = PatternShapeLibrary.ShapeType.Hexagon;
				return true;
			default:
				type = default;
				return false;
		}
	}
}
