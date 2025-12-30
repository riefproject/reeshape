# Slide 1 – Judul & Konteks
- Aplikasi desktop “Shape Playground” dikembangkan secara individual untuk memenuhi Evaluasi Tengah Semester Ganjil 2025 Politeknik Negeri Bandung pada mata kuliah Grafika Komputer.
- Produk ini dirancang sebagai implementasi Pattern Block Activity dengan fokus pada bentuk dasar dan transformasi 2D sesuai spesifikasi tugas.
- Presentasi mencakup pengantar kebutuhan, penjabaran teknis, serta demonstrasi aplikasi dalam satu alur terpadu.
- Tujuan utama sesi ialah memberikan gambaran menyeluruh mengenai nilai pembelajaran dan fondasi teknis yang direalisasikan.

# Slide 2 – Agenda & Alur Sesi
- Urutan sesi: pembukaan ringkas, penjelasan kebutuhan dan fitur utama, demonstrasi, kemudian diskusi.
- Setiap fitur diperkenalkan dari sisi konsep dan keterkaitannya dengan spesifikasi ETS sebelum ditunjukkan potongan kode yang relevan.
- Bagian demonstrasi menampilkan alur penggunaan aplikasi dari perspektif pengguna untuk menegaskan kesesuaian dengan tuntutan tugas.
- Waktu di akhir disediakan untuk pertanyaan, klarifikasi teknis, serta saran pengembangan lanjutan.

# Slide 3 – Visi & Tujuan Pembelajaran
- Tujuan pembelajaran menekankan pemahaman translasi, rotasi, dan penskalaan dalam grafika 2D melalui latihan interaktif.
- Aplikasi menjembatani konsep teoretis dan implementasi praktik sehingga transformasi tampak nyata bagi pengguna.
- Pendekatan belajar aktif diwujudkan melalui ringkasan materi, praktik langsung di stage, dan evaluasi progres.
- Fitur pembuatan pola memperkaya eksplorasi kreatif sekaligus menegaskan penguasaan bentuk dasar dan transformasi.

# Slide 4 – Fitur Utama Aplikasi
- Challenge berbasis outline 720p dengan tiga tema berbeda (Rocket, UFO, Astronaut) yang memanfaatkan bentuk dasar dan transformasi manual.
- Interaksi drag & drop, rotasi Q/E, translasi WASD/panah, serta reset shape memenuhi syarat input mouse dan keyboard.
- Snapping ke outline abu-abu, HUD progres, serta validasi kemenangan otomatis memberikan umpan balik belajar.
- Template Builder, galeri My Patterns, dan penyimpanan progres melengkapi kebutuhan pembuatan pola dan monitoring hasil.

# Slide 5 – Arsitektur Tingkat Tinggi
- Lapisan antarmuka mengelola halaman Welcome, PLAY, Guide, dan My Patterns sebagai gerbang navigasi pengguna.
- Lapisan gameplay bertanggung jawab atas logika stage, penyusunan shape, dan aturan snapping dengan implementasi transformasi sendiri.
- Data pola disimpan dalam berkas JSON sehingga konten tantangan dapat diperluas tanpa memodifikasi kode sumber.
- Seluruh komponen terhubung melalui layanan ringan, termasuk penyimpanan progres, yang memastikan pengalaman konsisten.

# Slide 6 – Menu Welcome & Navigasi
- Halaman Welcome menjadi pintu masuk yang langsung menyesuaikan latar dan elemen UI ketika dimuat.
- Daftar tombol navigasi mengarah ke About, PLAY, Guide, Settings, dan Exit untuk memenuhi kebutuhan interaksi berbasis mouse.
- Panel Settings menyediakan ringkasan progres sekaligus opsi untuk mereset data sebagai bagian dari monitoring hasil belajar.
- Halaman ini menjadi titik awal demonstrasi karena menampilkan status pengguna secara real time.

# Slide 7 – Katalog Tantangan
- Katalog tantangan mencakup stage Rocket, UFO, dan Astronaut dengan tingkat kesulitan bertahap sesuai syarat tiga challenge.
- Setiap entri menyimpan ringkasan tujuan, tips, dan metadata lain sebagai panduan awal pemain.
- Metadata dimanfaatkan ulang di berbagai antarmuka sehingga informasi stage tampil konsisten.
- Penambahan tantangan baru dapat dilakukan dengan menambahkan metadata dan berkas template sesuai format tugas.

# Slide 8 – Siklus Hidup Stage
- Pada saat stage dibuka, sistem menyusun layout 720p, palet shape, dan area kerja dalam keadaan bersih.
- Perubahan ukuran jendela dipantau agar penempatan HUD tetap proporsional serta mendukung bonus scaling.
- Cache dan status shape diinisialisasi ulang sehingga pengalaman setiap stage konsisten dan bebas residu.
- Ketika stage ditutup, resource dilepaskan untuk menjaga kestabilan sesi berikutnya.

# Slide 9 – Loop Input & Spawn Shape
- Loop input memantau peristiwa klik, drag, zoom, serta penekanan tombol keyboard tanpa memanfaatkan fungsi built-in transform Godot.
- Klik pada palet memunculkan shape baru di area kerja untuk segera diposisikan sesuai outline.
- Rotasi diatur dengan tombol Q/E sedangkan WASD atau panah mendukung translasi presisi, memenuhi syarat interaksi keyboard.
- Klik kanan mengembalikan shape ke palet sehingga percobaan ulang dapat dilakukan dengan cepat.

# Slide 10 – Snapping & Status Template
- Setiap template mendefinisikan slot target yang harus diisi shape dengan posisi dan orientasi tepat.
- Sistem menghitung jarak serta toleransi sudut untuk menentukan kapan shape menempel otomatis sebagai bonus snap.
- Status penyelesaian diperbarui melalui label indikator dan popup konfirmasi ketika seluruh outline terpenuhi.
- Mekanisme ini memberikan umpan balik visual yang jelas bahwa puzzle telah terselesaikan.

# Slide 11 – Intake Template JSON
- Berkas JSON menyimpan daftar bentuk, koordinat pivot, sudut, dan skala template yang menjadi dasar outline abu-abu.
- Saat dimuat, data dikonversi ke struktur internal agar siap diterapkan pada stage aktif.
- Proses serialisasi yang sama digunakan Template Builder ketika menyimpan pola baru sehingga format tetap seragam.
- Logging singkat membantu validasi keberhasilan proses muat ataupun simpan template.

# Slide 12 – Dynamic Pattern Stage Flow
- Stage dinamis memperoleh informasi template, nama tampilan, dan tingkat kesulitan melalui objek data sementara.
- Nama stage dipangkas otomatis agar sesuai dengan batasan ruang label HUD.
- Jumlah shape per tipe dihitung untuk menyusun palet serta kuota yang dibutuhkan.
- Label difficulty pada HUD mempertegas tingkat tantangan yang sedang dicoba.

# Slide 13 – Template Builder UI
- Template Builder menyediakan pengaturan skala global dan palet shape untuk komposisi bebas.
- Tombol Export membuka dialog penamaan kemudian menulis pola ke direktori template.
- Tombol Load menampilkan daftar template yang dapat diedit atau dijadikan titik awal.
- Pengaturan HUD dihitung ulang ketika dimensi jendela berubah supaya tetap nyaman dipakai.

# Slide 14 – Galeri My Patterns
- Halaman My Patterns menampilkan setiap pola buatan pengguna dalam bentuk tombol interaktif.
- Template default tidak ditampilkan sehingga fokus tertuju pada karya custom.
- Ketika tombol dipilih, aplikasi berpindah ke stage dinamis sambil memuat pola tersebut.
- Pesan informatif disediakan apabila belum ada pola tersimpan untuk mendorong eksplorasi.

# Slide 15 – Penyimpanan Progres
- Progres stage ditulis ke penyimpanan lokal agar catatan best time serta status selesai terjaga.
- Saat stage selesai, sistem membandingkan durasi terbaru dengan rekor sebelumnya sebelum memperbarui data.
- Pembacaan data menghasilkan salinan sehingga antarmuka tidak mengubah penyimpanan asli.
- Halaman Welcome selalu mengambil data mutakhir sehingga status progres tampil akurat.

# Slide 16 – Rencana Demo & Roadmap
- Rencana demo: menjelajah Welcome, memainkan Rocket Stage, membuka Guide, mencoba Template Builder, lalu menjalankan pola custom.
- Poin yang akan disorot meliputi mekanisme snapping, timer stage, serta pesan sukses saat ekspor JSON.
- Langkah pengembangan selanjutnya mencakup paginasi menu PLAY, penambahan bentuk baru, dan fitur berbagi template.
- Sesi ditutup dengan undangan untuk menyampaikan pertanyaan maupun ide pengembangan lanjutan.
