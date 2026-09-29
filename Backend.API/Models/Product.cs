namespace Backend.API.Models;
public class Product
{
    public int ProductId { get; set; }
    public string TenSP { get; set; } = null!;
    public string? MoTa { get; set; }
    public string? ThongSoKyThuat { get; set; }
    public string? HuongDanSuDung { get; set; }

    // Hạn sử dụng riêng theo từng sản phẩm (theo lô hàng hiện có trong kho)
    public DateTime? HanSuDung { get; set; }

    // Thời hạn bảo hành theo tháng (0 = không bảo hành)
    public int BaoHanhThang { get; set; } = 0;

    public decimal Gia { get; set; }
    public string? HinhAnh { get; set; }
    public bool TrangThai { get; set; } = true;
    public DateTime NgayTao { get; set; } = DateTime.Now;
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    public int SupplierId { get; set; }
    public Supplier Supplier { get; set; } = null!;
    public ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
    public Inventory? Inventory { get; set; }
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<ProductPromotion> ProductPromotions { get; set; } = new List<ProductPromotion>();
}
