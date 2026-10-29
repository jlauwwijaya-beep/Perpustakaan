namespace Perpustakaan.Model;

public class Peminjaman
{
    public int Id {get; set;}
    public int AnggotaId {get; set;}
    public int BukuId {get; set;}
    public DateTime TanggalPinjam {get; set;}
    public DateTime? TanggalKembali {get; set;}
    public string Alasan {get; set;} = "";
}