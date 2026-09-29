namespace Backend.API.Models;
public class Promotion
{
    public int PromotionId { get; set; }
    public string TenKM { get; set; } = null!;
    public string? MoTa { get; set; }
    public int PhanTramGiam { get; set; }
    public DateTime NgayBatDau { get; set; }
    public DateTime NgayKetThuc { get; set; }
    public bool TrangThai { get; set; } = true;
    public ICollection<ProductPromotion> ProductPromotions { get; set; } = new List<ProductPromotion>();
}