namespace Perpustakaan.Model;

public class Buku {
    
    public int Id {get; set;}
    public string Judul {get; set;} = "";
    public string Penulis {get; set;} = "";
    public int Stok {get; set;}
    public string Status {get; set;} = "Tersedia";

}