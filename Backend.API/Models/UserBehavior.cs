namespace Backend.API.Models;
public static class BehaviorType
{
    public const string Xem = "Xem";
    public const string TimKiem = "TimKiem";
    public const string Mua = "Mua";
    public const string DanhGia = "DanhGia";
}
public class UserBehavior
{
    public int BehaviorId { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public string LoaiHanhVi { get; set; } = null!;
    public DateTime ThoiGian { get; set; } = DateTime.Now;
    public double GiaTri { get; set; } = 1;
}