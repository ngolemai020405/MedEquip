using Backend.API.Data;
using Backend.API.DTOs;
using Backend.API.Models;
using Backend.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Backend.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly TokenService _tokenService;
    private readonly IMemoryCache _cache;
    private readonly EmailService _emailService;

    public AuthController(AppDbContext db, TokenService tokenService, IMemoryCache cache, EmailService emailService)
    {
        _db = db;
        _tokenService = tokenService;
        _cache = cache;
        _emailService = emailService;
    }
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest req)
    {
        var user = await _db.Users.Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Email == req.Email);
        if (user == null || !user.TrangThai)
            return Unauthorized(new { message = "Email hoac mat khau khong dung" });
        bool matKhauDung = BCrypt.Net.BCrypt.Verify(req.MatKhau, user.PasswordHash);
        if (!matKhauDung)
            return Unauthorized(new { message = "Email hoac mat khau khong dung" });
        var token = _tokenService.CreateToken(user, user.Role.RoleName);
        return Ok(new AuthResponse
        {
            Token = token,
            UserId = user.UserId,
            HoTen = user.HoTen,
            Email = user.Email,
            RoleName = user.Role.RoleName
        });
    }
    public class SendOtpRequest { public string Email { get; set; } = null!; }

    [HttpPost("send-otp")]
    public async Task<IActionResult> SendOtp(SendOtpRequest req)
    {
        if (await _db.Users.AnyAsync(u => u.Email == req.Email))
            return BadRequest(new { message = "Email da duoc su dung" });
            
        var otp = new Random().Next(100000, 999999).ToString();
        _cache.Set($"OTP_{req.Email}", otp, TimeSpan.FromMinutes(5));
        
        var sendSuccess = await _emailService.SendEmailAsync(req.Email, "Mã OTP đăng ký tài khoản MedEquip", $"<h2>Mã OTP của bạn là: <b>{otp}</b></h2><p>Mã có hiệu lực trong 5 phút.</p>");
        
        if (!sendSuccess)
        {
            Console.WriteLine($"[DEMO MODE] MÃ OTP CỦA {req.Email} LÀ: {otp}");
            return BadRequest(new { message = $"Không thể gửi email thực tế do chưa cấu hình SMTP. (Tính năng Demo) Mã OTP của bạn là: {otp}" });
        }
        
        return Ok(new { message = "Đã gửi mã OTP" });
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest req)
    {
        if (await _db.Users.AnyAsync(u => u.Email == req.Email))
            return BadRequest(new { message = "Email da duoc su dung" });

        var cachedOtp = _cache.Get<string>($"OTP_{req.Email}");
        if (cachedOtp == null || cachedOtp != req.Otp)
            return BadRequest(new { message = "Mã OTP không hợp lệ hoặc đã hết hạn" });
        var khachHangRole = await _db.Roles.FirstAsync(r => r.RoleName == "KhachHang");
        var user = new User
        {
            HoTen = req.HoTen,
            Email = req.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.MatKhau),
            SDT = req.SDT,
            DiaChi = req.DiaChi,
            TinhId = req.TinhId,
            PhuongXaId = req.PhuongXaId,
            SoNhaTenDuong = req.SoNhaTenDuong,
            RoleId = khachHangRole.RoleId,
            TrangThai = true
        };
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        var token = _tokenService.CreateToken(user, khachHangRole.RoleName);
        return Ok(new AuthResponse
        {
            Token = token,
            UserId = user.UserId,
            HoTen = user.HoTen,
            Email = user.Email,
            RoleName = khachHangRole.RoleName
        });
    }
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest req)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == req.Email);
        if (user == null)
        {
            return Ok(new { message = "Nếu email hợp lệ, mật khẩu mới sẽ được gửi vào hộp thư của bạn." });
        }
        
        var newPassword = new Random().Next(100000, 999999).ToString();
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
        await _db.SaveChangesAsync();
        
        var sendSuccess = await _emailService.SendEmailAsync(user.Email, "Mật khẩu mới tài khoản MedEquip", $"<h2>Mật khẩu mới của bạn là: <b>{newPassword}</b></h2><p>Vui lòng đăng nhập và đổi lại mật khẩu ngay.</p>");
        
        if (!sendSuccess)
        {
            Console.WriteLine($"[DEMO MODE] MẬT KHẨU MỚI CỦA {req.Email} LÀ: {newPassword}");
            return BadRequest(new { message = $"Không thể gửi email thực tế do chưa cấu hình SMTP. (Tính năng Demo) Mật khẩu mới là: {newPassword}" });
        }
        
        return Ok(new { message = "Mật khẩu mới đã được gửi vào email của bạn." });
    }
    [HttpGet("profile")]
    [Authorize]
    public async Task<IActionResult> GetProfile()
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var user = await _db.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.UserId == userId);
        if (user == null) return NotFound();
        return Ok(new
        {
            user.UserId,
            user.HoTen,
            user.Email,
            user.SDT,
            user.DiaChi,
            user.SoNhaTenDuong,
            user.TinhId,
            user.PhuongXaId,
            RoleName = user.Role.RoleName
        });
    }
    public class UpdateProfileRequest
    {
        public string Email { get; set; } = null!;
        public string HoTen { get; set; } = null!;
        public string? SDT { get; set; }
        public int? TinhId { get; set; }
        public int? PhuongXaId { get; set; }
        public string? SoNhaTenDuong { get; set; }
    }
    [HttpPut("profile")]
    [Authorize]
    public async Task<IActionResult> UpdateProfile(UpdateProfileRequest req)
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var user = await _db.Users.FindAsync(userId);
        if (user == null) return NotFound();
        if (req.Email != user.Email && await _db.Users.AnyAsync(u => u.Email == req.Email))
        {
            return BadRequest(new { message = "Email này đã được sử dụng bởi tài khoản khác." });
        }
        user.Email = req.Email;
        user.HoTen = req.HoTen;
        user.SDT = req.SDT;
        user.TinhId = req.TinhId;
        user.PhuongXaId = req.PhuongXaId;
        user.SoNhaTenDuong = req.SoNhaTenDuong;
        
        await _db.SaveChangesAsync();
        return Ok(new { success = true });
    }

    public class ChangePasswordRequest
    {
        public string OldPassword { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest req)
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var user = await _db.Users.FindAsync(userId);
        if (user == null) return NotFound();
        if (!BCrypt.Net.BCrypt.Verify(req.OldPassword, user.PasswordHash))
            return BadRequest(new { message = "Mật khẩu cũ không chính xác." });
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.NewPassword);
        await _db.SaveChangesAsync();
        return Ok(new { success = true });
    }
}