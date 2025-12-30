# reeshape - Shape Playground (Pattern Block Activity)

Note: For Indonesian documentation, see `README-id.md`.

Welcome to **reeshape**!
A desktop app built with Godot 4 (C#) by Arief F-sa Wijaya (241511002) for the 2025 midterm of the Computer Graphics course at Politeknik Negeri Bandung. The mission: revive the classic pattern block activity with modern interaction, precise controls, and a template pipeline you can export and replay.

## Showcase Highlights
- Manual 2D transforms (translate/rotate/scale) without relying on Godot's built-in transforms.
- Snap-based validation to 100% completion with win popup and best-time tracking.
- Three curated 720p challenges (Rocket, UFO, Astronaut) with precise outline targets.
- Hybrid controls: drag and drop, Q/E rotate, WASD/arrow micro-move, right-click reset.
- Template Builder + My Patterns: build -> export JSON -> replay.
- Persistent progress stored in `user://save.dat`.
- Modular stage/template system (TemplateShapeStage, TemplateLoader) with JSON receipts.

## What is reeshape?
Imagine a colorful block puzzle that comes alive on screen. You can drag base shapes, rotate, move, and assemble them into the correct silhouette. All transforms and snapping are implemented via custom scripts to make 2D computer graphics concepts tangible.

## Base Shapes and Transforms
Available shapes:
- Square
- Triangle
- Trapezoid
- Parallelogram / Rhombus
- Hexagon

Transforms include translation, rotation, global scaling, and snapping. Core logic is handled by `ShapePlayground` and the `TemplateShapeStage` family.

## Template Pipeline (Build -> Export -> Replay)
1. Template Builder: create your own pattern with base shapes.
2. Export JSON: save the pattern as a template.
3. My Patterns: replay your custom patterns anytime.

## How to Play
1. Open the app -> the Welcome page shows your latest progress.
2. Press PLAY -> choose Rocket/UFO/Astronaut or enter Template Builder.
3. Arrange shapes -> drag from the palette, rotate, and micro-move with keyboard.
4. Check progress -> HUD shows progress and a real-time timer.
5. Finish -> 100% completion triggers the win popup and saves best time.

## Folder Structure (Short)
```
├── Scenes/                 # Godot scenes (Welcome, PLAY/Projects, Stage, Builder, etc.)
├── Scripts/                # C# scripts
│   ├── Stages/             # Stage logic, templates, builder
│   ├── UI/                 # Pages and UI components
│   └── Utils/              # Helpers (TemplateLoader, StageSaveService, TimeFormatUtils)
├── StagesReceipt/          # JSON templates (Rocket, UFO, Astronaut, and custom)
└── project.godot           # Godot project config
```

## Tech and Requirements
- Godot Engine 4.x (C#/.NET)
- Target platform: Desktop (Windows, macOS, Linux)
- Game resolution: 1280 x 720

## Run Instructions
1. Install Godot 4.x with C# support.
2. Clone this repository or extract the submitted zip.
3. Open the project in Godot (`project.godot`).
4. Run the `Scenes/Pages/Welcome.tscn` scene.
5. Have fun!

## Roadmap
- Add background music and sound effects.
- Refresh UI/UX to better match stage themes.
- Add more base shapes and pagination on the PLAY page.
- Prepare template sharing between users.

## Thanks
Thanks for stopping by. I hope reeshape becomes a fun reference for learning 2D computer graphics.
