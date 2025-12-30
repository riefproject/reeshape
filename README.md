# Shape Playground – Pattern Block Activity versi Godot

Selamat datang di Shape Playground! 🎮  
Ini adalah aplikasi desktop buatan saya, Arief F-sa Wijaya (241511002), sebagai pemenuhan Evaluasi Tengah Semester Ganjil 2025 untuk mata kuliah Komputer Grafik di Politeknik Negeri Bandung. Tujuannya? Menghidupkan lagi “pattern block activity” dengan cara yang lebih modern dan interaktif.

## 👩‍🏫 Apa itu Shape Playground?
Bayangkan main puzzle dengan balok-balok warna, tapi semuanya hidup di layar. Kita bisa menyeret bentuk dasar, memutar, menggeser, dan menyusunnya hingga membentuk siluet yang menarik. Semua transformasi dilakukan memakai skrip sendiri (tanpa mengandalkan built-in transform Godot), sehingga sekaligus jadi ajang latihan konsep grafika komputer 2D.

## ✨ Fitur Utama

- **Tiga Tantangan 720p**  
  Rocket, UFO, dan Astronaut hadir dengan outline abu-abu yang menunggu untuk diisi. Semua template dirancang menggunakan fungsi bentuk dasar yang sudah dibahas di praktikum sebelumnya.

- **Kontrol Mouse + Keyboard**  
  Drag & drop untuk spawn dan geser shape, rotasi pakai Q/E, translasi presisi dengan WASD atau tombol panah, serta klik kanan untuk mengembalikan shape ke palet. Semuanya responsif dan fun.

- **Snapping & Validasi Otomatis**  
  Bentuk akan “klik” ke tempatnya ketika mendekati outline yang tepat. Setelah semua slot terisi, HUD menampilkan progres 100%, popup kemenangan muncul, dan waktu terbaik otomatis tersimpan.

- **Template Builder & Galeri Custom**  
  Bikin pola sendiri lewat Template Builder, ekspor ke JSON, lalu mainkan kembali melalui halaman My Patterns. Cocok buat latihan tambahan atau ajang pamer kreativitas.

- **HUD dan Penyimpanan Progres**  
  Timer real-time, indikator difficulty, serta daftar progres di halaman Welcome. Data disimpan ke `user://save.dat` agar rekor tetap aman walau aplikasi ditutup.

## 🧱 Bentuk Dasar & Transformasi
Bentuk yang tersedia antara lain:

- Persegi
- Segitiga
- Trapesium
- Jajar Genjang / Belah Ketupat
- Hexagon

Transformasinya meliputi translasi, rotasi, scaling global, dan snapping. Semua diatur lewat script `ShapePlayground` dan keluarga `TemplateShapeStage`.

## 🎯 Cara Main

1. **Buka aplikasi** → Halaman Welcome tampil dengan progres terbaru.  
2. **Tekan PLAY** → Tentukan tantangan (Rocket/UFO/Astronaut) atau masuk ke Template Builder.  
3. **Susun bentuk** → Drag shape dari palet, posisikan di outline, pakai keyboard untuk rotasi/gerakan presisi.  
4. **Cek progres** → Lihat persentase di HUD. Begitu 100%, popup kemenangan muncul dan rekor tersimpan.  
5. **Eksperimen** → Coba Template Builder, buat pola baru, ekspor JSON, lalu mainkan lagi di My Patterns.

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
4. Jalankan scene `Scenes/Pages/Welcome.tscn` atau mainkan langsung dari editor.  
5. Selamat bersenang-senang!

## 📈 Rencana Pengembangan

- Menambahkan musik latar dan efek suara.  
- Menata ulang UI/UX supaya lebih senada dengan tema stage.  
- Menambahkan bentuk dasar baru dan paginasi di halaman PLAY.  
- Mempersiapkan fitur berbagi template antar pengguna.

## 🙏 Terima Kasih
Terima kasih sudah mampir. Kalau ingin memberikan masukan atau bertukar ide, silakan hubungi saya di kampus atau lewat platform yang biasa kita gunakan. Semoga Shape Playground bisa jadi referensi seru untuk belajar grafika komputer!
