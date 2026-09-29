namespace Backend.API.Models;
public class User
{
    public int UserId { get; set; }
    public string HoTen { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? SDT { get; set; }
    public string PasswordHash { get; set; } = null!;

    // Địa chỉ mặc định (tự điền khi đặt hàng) - 2 cấp: Tỉnh -> Xã/Phường
    public string? SoNhaTenDuong { get; set; }
    public int? TinhId { get; set; }
    public Tinh? Tinh { get; set; }
    public int? PhuongXaId { get; set; }
    public PhuongXa? PhuongXa { get; set; }

    [System.Obsolete("Dùng SoNhaTenDuong + TinhId/PhuongXaId thay cho chuỗi tự do này")]
    public string? DiaChi { get; set; }

    public string? AvatarUrl { get; set; }
    public bool TrangThai { get; set; } = true;
    public DateTime NgayTao { get; set; } = DateTime.Now;
    public int RoleId { get; set; }
    public Role Role { get; set; } = null!;
}
