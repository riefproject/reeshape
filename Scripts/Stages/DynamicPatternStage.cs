using Godot;
using System.Collections.Generic;
using ShapeType = PatternShapeLibrary.ShapeType;

public partial class DynamicPatternStage : TemplateShapeStage {
	private static readonly (ShapeType Type, string Label)[] PaletteOrder = {
		(ShapeType.Segitiga, "Segitiga"),
		(ShapeType.Trapesium, "Trapesium"),
		(ShapeType.JajarGenjang, "Jajar Genjang"),
		(ShapeType.JajarGenjang2, "Belah Ketupat"),
		(ShapeType.Persegi, "Persegi"),
		(ShapeType.Hexagon, "Hexagon")
	};

	public override void _Ready() {
		// Ambil data dari DynamicPatternData sebelum base._Ready()
		string templatePath = DynamicPatternData.TemplatePath;
		string patternName = DynamicPatternData.PatternName;
		string difficulty = DynamicPatternData.Difficulty;
		string returnScene = DynamicPatternData.ReturnScene;

		if (!string.IsNullOrEmpty(templatePath)) {
			ReceiptPath = templatePath;
			
			// Generate stage ID dari pattern name
			string baseId = System.IO.Path.GetFileNameWithoutExtension(templatePath).ToLowerInvariant();
			StageId = baseId;
			
			// Truncate pattern name untuk display (max 10 karakter untuk stage label)
			StageDisplayName = TruncateText(patternName, 10);
			
			// Store difficulty and return scene
			DynamicPatternData.ActiveDifficulty = difficulty;
			DynamicPatternData.ActiveReturnScene = returnScene;
		}

		base._Ready();
	}

	// Tujuan: truncate text jika melebihi max length
	// Input: text adalah string yang akan di-truncate, maxLength adalah panjang maksimal
	// Output: string yang sudah di-truncate dengan "..." jika perlu
	private string TruncateText(string text, int maxLength)
	{
		if (string.IsNullOrEmpty(text) || text.Length <= maxLength)
		{
			return text;
		}

		// Ambil karakter awal + "..." (7 karakter + ... untuk max 10, atau proportional)
		int takeChars = Mathf.Max(7, maxLength - 3);
		return text.Substring(0, takeChars) + "...";
	}

	protected override void OnTemplateLoaded() {
		base.OnTemplateLoaded();
		if (Template == null) return;

		// Update stage name dari JSON jika tersedia (DENGAN TRUNCATION!)
		if (!string.IsNullOrEmpty(Template.Name)) {
			string truncatedName = TruncateText(Template.Name, 10);
			
			// Update StageDisplayName untuk consistency
			StageDisplayName = truncatedName;
			
			// Update stage label langsung
			var stageLabel = GetNodeOrNull<Label>("StageLabel");
			if (stageLabel != null) {
				stageLabel.Text = truncatedName;
			}
			
			GD.Print($"[DynamicPatternStage] Using JSON name: '{Template.Name}' (displayed as: '{truncatedName}')");
		}

		var counts = CountShapesFromTemplate();
		if (counts.Count == 0) return;

		var definition = BuildStageDefinition(counts);
		ApplyStage(definition);
		SetGlobalShapeScale(TemplateScaleApplied);
		UpdateStatusLabel();

		// Update difficulty label
		UpdateDifficultyLabel(DynamicPatternData.ActiveDifficulty);
		
		QueueRedraw();
	}

	private Dictionary<ShapeType, int> CountShapesFromTemplate() {
		var counts = new Dictionary<ShapeType, int>();
		foreach (var entry in Template.Entries) {
			counts.TryGetValue(entry.ShapeType, out int current);
			counts[entry.ShapeType] = current + 1;
		}
		return counts;
	}

	private ShapeStageDefinition BuildStageDefinition(Dictionary<ShapeType, int> counts) {
		return CreateStageDefinitionFromCounts(counts, PaletteOrder);
	}

	private void UpdateDifficultyLabel(string difficulty) {
		var bestTimeLabel = GetNodeOrNull<Label>("BestTimeLabel");
		if (bestTimeLabel != null) {
			bestTimeLabel.Text = difficulty;
		}
	}

	protected override void OnTemplateCompletionChanged(bool completed) {
		if (completed) GD.Print($"[DynamicPatternStage] Pattern '{DynamicPatternData.PatternName}' complete!");
		base.OnTemplateCompletionChanged(completed);
	}
}

// Static class untuk pass data between scenes
public static class DynamicPatternData
{
	public static string TemplatePath { get; set; } = "";
	public static string PatternName { get; set; } = "";
	public static string Difficulty { get; set; } = "Custom Pattern";
	public static string ReturnScene { get; set; } = "res://Scenes/Pages/Welcome.tscn";
	
	// Active state (set setelah Ready)
	public static string ActiveDifficulty { get; set; } = "";
	public static string ActiveReturnScene { get; set; } = "";
}
