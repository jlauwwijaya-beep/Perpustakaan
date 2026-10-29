// Memanggil namespace milik library yang tadi dipasang
// Ada supaya kita bisa memakai SqliteConnection di bawah
using Microsoft.Data.Sqlite;

// Connection string: teks yang memberi tahu library di mana file database berada
// "Data Source=" = penanda lokasi; perpustakaan.db = nama file database kita
// Ada karena library butuh tahu file mana yang harus dibuka
var connectionString = "Data Source=perpustakaan.db";

// Membuat objek sambungan ke database memakai connection string tadi
// "using var" = sambungan otomatis ditutup saat program selesai memakainya
// Ada supaya sambungan tidak tertinggal terbuka dan menghabiskan sumber daya
using var connection = new SqliteConnection(connectionString);

// Membuka sambungan ke database
// Ada karena objek tadi baru "dibuat", belum benar-benar tersambung
// Kalau file perpustakaan.db belum ada, SQLite otomatis membuatkannya di sini
connection.Open();
// Membuat objek perintah yang akan dikirim ke database lewat sambungan yang sudah terbuka
// Ada karena sambungan saja hanya jalur, perintah SQL perlu "pembawa" untuk dikirim
var command = connection.CreateCommand();

// Mengisi teks perintah dengan SQL pembuat tiga tabel
// """ ... """ = cara menulis teks panjang beberapa baris di C#
command.CommandText = """
    -- Mengaktifkan penjagaan foreign key di sambungan ini
    -- Ada karena SQLite secara bawaan TIDAK memeriksa foreign key kalau tidak diminta
    PRAGMA foreign_keys = ON;

    -- IF NOT EXISTS = buat tabel hanya kalau belum ada
    -- Ada supaya program boleh dijalankan berkali-kali tanpa error
    CREATE TABLE IF NOT EXISTS Buku (
        -- Primary key yang nilainya naik sendiri (1, 2, 3, ...)
        Id INTEGER PRIMARY KEY AUTOINCREMENT,
        -- NOT NULL = kolom ini wajib diisi, tidak boleh kosong
        Judul TEXT NOT NULL,
        Penulis TEXT NOT NULL,
        Stok INTEGER NOT NULL,
        -- DEFAULT 'Tersedia' = kalau tidak diisi, otomatis bernilai Tersedia (sama dengan model C#)
        Status TEXT NOT NULL DEFAULT 'Tersedia'
    );

    -- Tabel kedua: Anggota
    CREATE TABLE IF NOT EXISTS Anggota (
        Id INTEGER PRIMARY KEY AUTOINCREMENT,
        Nama TEXT NOT NULL,
        Jenkel TEXT NOT NULL
    );

    -- Tabel ketiga: Peminjaman, yang menghubungkan Anggota dan Buku
    CREATE TABLE IF NOT EXISTS Peminjaman (
        Id INTEGER PRIMARY KEY AUTOINCREMENT,
        -- Foreign key: menyimpan Id anggota yang meminjam
        AnggotaId INTEGER NOT NULL,
        -- Foreign key: menyimpan Id buku yang dipinjam
        BukuId INTEGER NOT NULL,
        TanggalPinjam TEXT NOT NULL,
        -- Tanpa NOT NULL = boleh kosong, karena baru terisi saat buku dikembalikan
        TanggalKembali TEXT,
        Alasan TEXT,
        -- Menyatakan AnggotaId harus menunjuk ke Id yang ada di tabel Anggota
        FOREIGN KEY (AnggotaId) REFERENCES Anggota(Id),
        -- Menyatakan BukuId harus menunjuk ke Id yang ada di tabel Buku
        FOREIGN KEY (BukuId) REFERENCES Buku(Id)
    );
    """;

// Menjalankan perintah SQL tadi di database
// ExecuteNonQuery = untuk perintah yang tidak mengembalikan data (seperti membuat tabel)
// Ada supaya ketiga tabel benar-benar dibuat
command.ExecuteNonQuery();

// Menampilkan bukti bahwa pembuatan tabel selesai tanpa error
Console.WriteLine("Tabel berhasil dibuat");

// Mengganti isi perintah dengan SQL yang memasukkan data contoh
// Objek command yang sama dipakai lagi, kita hanya mengganti teks perintahnya
// Ada supaya tidak perlu membuat objek perintah baru untuk setiap pekerjaan
command.CommandText = """
    -- Menambah dua buku; Id tidak ditulis karena terisi otomatis oleh AUTOINCREMENT
    -- Status tidak ditulis karena otomatis bernilai Tersedia (DEFAULT)
    INSERT INTO Buku (Judul, Penulis, Stok) VALUES ('Belajar C#', 'Andi', 5);
    INSERT INTO Buku (Judul, Penulis, Stok) VALUES ('Belajar Blazor', 'Budi', 3);

    -- Menambah satu anggota
    INSERT INTO Anggota (Nama, Jenkel) VALUES ('Citra', 'Perempuan');

    -- Menambah satu peminjaman: anggota nomor 1 meminjam buku nomor 1
    -- AnggotaId dan BukuId berisi Id yang sudah ada, kalau tidak, foreign key menolaknya
    INSERT INTO Peminjaman (AnggotaId, BukuId, TanggalPinjam, Alasan)
    VALUES (1, 1, '2026-10-29', 'Untuk belajar');
    """;

// Menjalankan perintah INSERT tadi
// Ada supaya data contoh benar-benar tersimpan di file database
command.ExecuteNonQuery();

// Mengganti perintah dengan SQL yang menghitung jumlah baris di tabel Buku
command.CommandText = "SELECT COUNT(*) FROM Buku;";

// ExecuteScalar = menjalankan perintah dan mengambil SATU nilai hasilnya
// Ada karena kita hanya butuh satu angka (jumlah buku), bukan seluruh tabel
var jumlahBuku = command.ExecuteScalar();

// Menampilkan jumlah buku sebagai bukti data masuk
Console.WriteLine("Jumlah buku: " + jumlahBuku);

Console.WriteLine("Database terhubung");