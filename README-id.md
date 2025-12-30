# reeshape — Shape Playground (Pattern Block Activity)

Selamat datang di **reeshape**! 🎮  
Aplikasi desktop berbasis Godot 4 (C#) buatan saya, Arief F-sa Wijaya (241511002), sebagai pemenuhan ETS Ganjil 2025 mata kuliah Komputer Grafik Politeknik Negeri Bandung. Misinya: menghidupkan “pattern block activity” dengan interaksi modern, kontrol presisi, dan pipeline template yang bisa diekspor serta dimainkan ulang.

## ⚡ Showcase Highlights
- **Transformasi 2D manual** (translate/rotate/scale) tanpa built-in transform Godot.  
- **Snapping + validasi otomatis** sampai 100% completion, lengkap win popup dan best-time.  
- **3 challenge 720p** (Rocket, UFO, Astronaut) dengan outline target presisi.  
- **Kontrol hybrid**: drag & drop, Q/E rotate, WASD/arrow micro-move, right-click reset.  
- **Template Builder + My Patterns**: build → export JSON → replay.  
- **Progress persisten** tersimpan di `user://save.dat`.  
- **Stage/template modular** (TemplateShapeStage, TemplateLoader) dengan JSON receipt.  

## 👩‍🏫 Apa itu reeshape?
Bayangkan puzzle balok warna yang hidup di layar. Kamu bisa menyeret bentuk dasar, memutar, menggeser, lalu menyusunnya jadi siluet yang tepat. Semua transformasi dan snapping diatur lewat script sendiri sebagai latihan konsep grafika komputer 2D secara nyata.

## 🧱 Bentuk Dasar & Transformasi
Bentuk yang tersedia:
- Persegi
- Segitiga
- Trapesium
- Jajar Genjang / Belah Ketupat
- Hexagon

Transformasi mencakup translasi, rotasi, scaling global, dan snapping. Core logic diatur lewat `ShapePlayground` dan keluarga `TemplateShapeStage`.

## 🧩 Template Pipeline (Build → Export → Replay)
1. **Template Builder**: susun pola sendiri dengan bentuk dasar.  
2. **Export JSON**: simpan pola sebagai template.  
3. **My Patterns**: mainkan ulang pola custom kapan saja.  

## 🎯 Cara Main
1. **Buka aplikasi** → Halaman Welcome tampil dengan progres terbaru.  
2. **Tekan PLAY** → Pilih Rocket/UFO/Astronaut atau masuk ke Template Builder.  
3. **Susun bentuk** → Drag dari palet, rotasi, geser presisi dengan keyboard.  
4. **Cek progres** → HUD menunjukkan progress dan timer real-time.  
5. **Selesai** → 100% completion memicu win popup + best-time tersimpan.  

## 📁 Struktur Folder Singkat
```
├── Scenes/                 # Scene Godot (Welcome, PLAY/Projects, Stage, Builder, dll.)
├── Scripts/                # C# scripts
│   ├── Stages/             # Logika stage, template, builder
│   ├── UI/                 # Halaman dan komponen antarmuka
│   └── Utils/              # Helper (TemplateLoader, StageSaveService, TimeFormatUtils)
├── StagesReceipt/          # Template JSON (Rocket, UFO, Astronaut, dan custom)
└── project.godot           # Konfigurasi proyek Godot
```

## 🛠️ Teknologi & Prasyarat
- Godot Engine 4.x (C#/.NET)  
- Target platform: Desktop (Windows, macOS, Linux)  
- Resolusi permainan: 1280 x 720  

## 🚀 Cara Menjalankan
1. Pastikan Godot 4.x dengan dukungan C# sudah terpasang.  
2. Clone repository ini atau ekstrak zip hasil pengumpulan ETS.  
3. Buka proyek melalui Godot (`project.godot`).  
4. Jalankan scene `Scenes/Pages/Welcome.tscn`.  
5. Selamat bersenang-senang!  

## 📈 Rencana Pengembangan
- Menambahkan musik latar dan efek suara.  
- Menata ulang UI/UX supaya lebih senada dengan tema stage.  
- Menambahkan bentuk dasar baru dan paginasi di halaman PLAY.  
- Mempersiapkan fitur berbagi template antar pengguna.  

## 🙏 Terima Kasih
Terima kasih sudah mampir. Semoga reeshape bisa jadi referensi seru untuk belajar grafika komputer 2D!
