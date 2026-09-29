using System.ComponentModel.DataAnnotations;
namespace Backend.API.DTOs;
public class LoginRequest
{
    [Required(ErrorMessage = "Vui lòng nhập Email")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    public string Email { get; set; } = null!;
    
    [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
    public string MatKhau { get; set; } = null!;
}
public class RegisterRequest
{
    [Required(ErrorMessage = "Họ tên là bắt buộc")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Họ tên từ 3-100 ký tự")]
    public string HoTen { get; set; } = null!;
    
    [Required(ErrorMessage = "Email là bắt buộc")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    public string Email { get; set; } = null!;
    
    [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
    [MinLength(6, ErrorMessage = "Mật khẩu tối thiểu 6 ký tự")]
    public string MatKhau { get; set; } = null!;
    
    [RegularExpression(@"^(0(3[2-9]|5[5689]|7[06789]|8[1-9]|9[0-46-9]))[0-9]{7}$",
        ErrorMessage = "Số điện thoại không đúng định dạng số di động Việt Nam (VD: 0912345678)")]
    public string? SDT { get; set; }
    public string? DiaChi { get; set; }
    
    public int? TinhId { get; set; }
    public int? PhuongXaId { get; set; }
    public string? SoNhaTenDuong { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập mã OTP")]
    public string Otp { get; set; } = null!;
}
public class AuthResponse
{
    public string Token { get; set; } = null!;
    public int UserId { get; set; }
    public string HoTen { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string RoleName { get; set; } = null!;
}
public class ForgotPasswordRequest
{
    public string Email { get; set; } = null!;
}