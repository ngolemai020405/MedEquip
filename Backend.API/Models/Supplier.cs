namespace Backend.API.Models;
public class Supplier
{
    public int SupplierId { get; set; }
    public string TenNCC { get; set; } = null!;
    public string? DiaChi { get; set; }
    public string? SDT { get; set; }
    public string? Email { get; set; }
    public string? NguoiLienHe { get; set; }
    public ICollection<Product> Products { get; set; } = new List<Product>();
}