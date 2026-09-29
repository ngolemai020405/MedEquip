using WebAdmin.Models;
using WebAdmin.Services;
using Microsoft.AspNetCore.Mvc;
namespace WebAdmin.Controllers;
public class AccountController : Controller
{
    private readonly ApiClient _api;
    public AccountController(ApiClient api) => _api = api;
    [HttpGet]
    public IActionResult Login()
    {
        if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Token")))
            return RedirectToAction("Index", "Dashboard");
        return View();
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginVM vm)
    {
        var (thanhCong, loi, data) = await _api.PostAsync<AuthResponseVM>("api/auth/login", vm);
        if (!thanhCong || data == null)
        {
            ViewBag.Loi = "Đăng nhập thất bại. Kiểm tra lại email/mật khẩu.";
            return View(vm);
        }
        if (data.RoleName == "KhachHang")
        {
            ViewBag.Loi = "Tai khoan khach hang khong duoc phep dang nhap trang quan tri.";
            return View(vm);
        }
        HttpContext.Session.SetString("Token", data.Token);
        HttpContext.Session.SetString("HoTen", data.HoTen);
        HttpContext.Session.SetString("RoleName", data.RoleName);
        HttpContext.Session.SetInt32("UserId", data.UserId);
        return RedirectToAction("Index", "Dashboard");
    }
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }
    [HttpGet]
    public IActionResult ForgotPassword()
    {
        return View();
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(string email)
    {
        if (string.IsNullOrEmpty(email))
        {
            ViewBag.Loi = "Vui lòng nhập email.";
            return View();
        }
        var (thanhCong, loi, data) = await _api.PostAsync<dynamic>("api/auth/forgot-password", new { Email = email });
        ViewBag.ThanhCong = "Nếu email tồn tại, mật khẩu đã được reset về 123456 (Demo).";
        return View();
    }
    [HttpGet]
    public async Task<IActionResult> Profile()
    {
        if (HttpContext.Session.GetString("Token") == null)
            return RedirectToAction("Login");
        var profile = await _api.GetAsync<Newtonsoft.Json.Linq.JObject>("api/auth/profile");
        if (profile == null) return RedirectToAction("Login");
        return View(profile);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Profile(string email, string hoTen, string? sdt, string? diaChi)
    {
        var (ok, loi) = await _api.PutAsync("api/auth/profile", new { Email = email, HoTen = hoTen, SDT = sdt, DiaChi = diaChi });
        if (ok)
        {
            TempData["Thanhcong"] = "Cập nhật hồ sơ thành công.";
            HttpContext.Session.SetString("HoTen", hoTen);
        }
        else
        {
            TempData["Loi"] = loi ?? "Cập nhật thất bại. Email có thể đã tồn tại.";
        }
        return RedirectToAction("Profile");
    }
}
