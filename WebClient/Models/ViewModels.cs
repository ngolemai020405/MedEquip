namespace WebClient.Models;
using System.ComponentModel.DataAnnotations;
public class LoginVM
{
    public string Email { get; set; } = null!;
    public string MatKhau { get; set; } = null!;
}
public class RegisterVM
{
    public string HoTen { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string MatKhau { get; set; } = null!;
    public string? SDT { get; set; }
    public string? DiaChi { get; set; }
    
    public int? TinhId { get; set; }
    public int? PhuongXaId { get; set; }
    public string? SoNhaTenDuong { get; set; }
    public string Otp { get; set; } = null!;
}
public class AuthResponseVM
{
    public string Token { get; set; } = null!;
    public int UserId { get; set; }
    public string HoTen { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string RoleName { get; set; } = null!;
}
public class CheckoutVM
{
    [Required(ErrorMessage = "Vui lòng nhập số nhà, tên đường")]
    public string SoNhaTenDuongGiao { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng chọn Tỉnh/Thành phố")]
    public int TinhId { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn Xã/Phường")]
    public int PhuongXaId { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập số điện thoại nhận hàng")]
    [RegularExpression(@"^(0(3[2-9]|5[5689]|7[06789]|8[1-9]|9[0-46-9]))[0-9]{7}$",
        ErrorMessage = "Số điện thoại không đúng định dạng số di động Việt Nam (VD: 0912345678)")]
    public string SDTNhan { get; set; } = null!;

    [Range(typeof(bool), "true", "true", ErrorMessage = "Bạn phải xác nhận đây là thông tin chính chủ trước khi đặt hàng")]
    public bool XacNhanChinhChu { get; set; }

    public string? GhiChu { get; set; }
    public string PhuongThucThanhToan { get; set; } = "COD";
    public string PhuongThucGiaoHang { get; set; } = "GiaoHangTieuChuan";
}
public class CartItem
{
    public int ProductId { get; set; }
    public string TenSP { get; set; } = null!;
    public decimal Gia { get; set; }
    public string? HinhAnh { get; set; }
    public int SoLuong { get; set; }
    public decimal ThanhTien => Gia * SoLuong;
}
public class Cart
{
    public List<CartItem> Items { get; set; } = new();
    public decimal TongTien => Items.Sum(i => i.ThanhTien);
}