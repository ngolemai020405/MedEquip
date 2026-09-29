namespace Backend.API.Models;
public class Review
{
    public int ReviewId { get; set; }
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public int SoSao { get; set; }
    public string? NoiDung { get; set; }
    public DateTime NgayDanhGia { get; set; } = DateTime.Now;
    public bool DaDuyet { get; set; } = true;
    public string? PhanHoi { get; set; }
    public DateTime? NgayPhanHoi { get; set; }
}