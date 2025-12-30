# Shape Playground – Pattern Block Activity Goes Digital

Welcome to Shape Playground! 🎮  
This desktop application was built by Arief F-sa Wijaya (241511002) as a solo submission for the Midterm Evaluation (ETS) 2025 in the Computer Graphics course at Politeknik Negeri Bandung. The idea is simple: revive the classic “pattern block activity” experience, but powered by Godot and C#.

## 👩‍🏫 What Is Shape Playground?
Think of a puzzle table filled with colorful blocks, except everything happens on-screen. You drag basic shapes, rotate them, snap them into position, and watch a silhouette come to life. All transformations are implemented through handwritten scripts—no Godot built-ins—so it doubles as a hands-on laboratory for 2D computer graphics concepts.

## ✨ Key Features

- **Three 720p Challenges**  
  Rocket, UFO, and Astronaut templates with grey outlines. Each layout was crafted using the same primitive functions practiced in earlier lab sessions.

- **Mouse + Keyboard Controls**  
  Drag & drop, Q/E rotation, WASD/arrow translation, and right-click to refund shapes. Everything feels responsive and satisfying.

- **Snapping & Auto Validation**  
  Shapes click into place when they match the outline. Once every slot is filled, a HUD indicator hits 100%, a victory popup appears, and the best time is saved automatically.

- **Template Builder & Custom Gallery**  
  Design your own template with the Builder, export it to JSON, and replay it through the My Patterns gallery. Perfect for extra practice or friendly show-and-tell.

- **HUD and Progress Persistence**  
  Real-time timer, difficulty badges, and a progress list on the Welcome page. Records are saved to `user://save.dat`, so your achievements stay intact across sessions.

## 🧱 Shape Palette & Transformations
Available primitives:

- Square
- Triangle
- Trapezoid
- Parallelogram / Rhombus
- Hexagon

Transformations include translation, rotation, global scaling, and snapping—handled by the `ShapePlayground` base class and the `TemplateShapeStage` family.

## 🎯 How to Play

1. **Launch the app** → The Welcome page shows your latest progress.  
2. **Hit PLAY** → Choose a challenge (Rocket/UFO/Astronaut) or jump into the Template Builder.  
3. **Arrange shapes** → Drag from the palette, align with the outline, use the keyboard for precision.  
4. **Monitor completion** → Watch the HUD percentage. When it hits 100%, enjoy the victory popup and saved record.  
5. **Experiment** → Create a custom template, export it, and replay it via My Patterns.

## 📁 Project Structure Snapshot

```
├── Scenes/                 # Godot scenes (Welcome, PLAY/Projects, Stage, Builder, etc.)
├── Scripts/                # C# scripts
│   ├── Stages/             # Stage logic, templates, builder
│   ├── UI/                 # UI pages and components
│   └── Utils/              # Helpers (TemplateLoader, StageSaveService, TimeFormatUtils)
├── StagesReceipt/          # Template JSON files (Rocket, UFO, Astronaut, custom)
└── project.godot           # Godot project configuration
```

## 🛠️ Stack & Requirements

- Godot Engine 4.x (C#/.NET)
- Target platform: desktop (Windows, macOS, Linux)
- Gameplay resolution: 1280 x 720

## 🚀 Getting Started

1. Install Godot 4.x with C# support.  
2. Clone this repository or unzip the ETS submission.  
3. Open `project.godot` with Godot.  
4. Run `Scenes/Pages/Welcome.tscn` or start the project directly.  
5. Have fun assembling shapes!

## 📈 Roadmap Ideas

- Layer in background music and sound effects.  
- Refresh the UI/UX to better match each stage’s theme.  
- Add new primitive shapes and pagination to the PLAY screen.  
- Prepare optional sharing features for custom templates.

## 🙏 Thank You!
Thanks for checking out Shape Playground. Feel free to reach out if you’d like to swap ideas or suggest improvements—whether you’re a fellow student or a curious explorer. Hope this project inspires your own adventures in 2D computer graphics!
