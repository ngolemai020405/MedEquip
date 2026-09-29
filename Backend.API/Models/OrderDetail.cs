namespace Backend.API.Models;
public class OrderDetail
{
    public int OrderDetailId { get; set; }
    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public int SoLuong { get; set; }
    public decimal DonGia { get; set; }
}