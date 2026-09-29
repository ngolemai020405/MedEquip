using WebClient.Models;
using WebClient.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
namespace WebClient.Controllers;
public class AccountController : Controller
{
    private readonly ApiClient _api;
    public AccountController(ApiClient api) => _api = api;

    private string ParseApiError(string? loiOriginal, string defaultMsg)
    {
        if (string.IsNullOrEmpty(loiOriginal)) return defaultMsg;
        try
        {
            var jObj = JObject.Parse(loiOriginal);
            if (jObj["errors"] != null)
            {
                var errors = jObj["errors"]!.Values().SelectMany(v => v.Select(e => e.ToString())).ToList();
                return string.Join("<br/>", errors);
            }
            if (jObj["message"] != null) return jObj["message"]!.ToString();
        }
        catch { }
        return loiOriginal;
    }
    [HttpGet]
    public IActionResult Login(string? returnUrl)
    {
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginVM vm, string? returnUrl)
    {
        var (thanhCong, loi, data) = await _api.PostAsync<AuthResponseVM>("api/auth/login", vm);
        if (!thanhCong || data == null)
        {
            ViewBag.Loi = ParseApiError(loi, "Email hoac mat khau khong dung.");
            return View(vm);
        }
        HttpContext.Session.SetString("Token", data.Token);
        HttpContext.Session.SetString("HoTen", data.HoTen);
        HttpContext.Session.SetInt32("UserId", data.UserId);
        if (!string.IsNullOrEmpty(returnUrl)) return LocalRedirect(returnUrl);
        return RedirectToAction("Index", "Home");
    }
    [HttpGet]
    public async Task<IActionResult> Register()
    {
        ViewBag.DanhSachTinh = await _api.GetAsync<List<JObject>>("api/diachi/tinh") ?? new List<JObject>();
        return View();
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterVM vm)
    {
        var (thanhCong, loi, data) = await _api.PostAsync<AuthResponseVM>("api/auth/register", vm);
        if (!thanhCong || data == null)
        {
            ViewBag.DanhSachTinh = await _api.GetAsync<List<JObject>>("api/diachi/tinh") ?? new List<JObject>();
            ViewBag.Loi = ParseApiError(loi, "Đăng ký thất bại. Email có thể đã tồn tại hoặc thông tin không hợp lệ.");
            return View(vm);
        }
        TempData["ToastMessage"] = "Đăng ký thành công, vui lòng đăng nhập.";
        return RedirectToAction("Login", "Account");
    }
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
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
        ViewBag.ThanhCong = "Nếu email tồn tại trong hệ thống, mật khẩu của bạn đã được khởi tạo lại thành 123456 (Tính năng Demo).";
        return View();
    }
    [HttpGet]
    public async Task<IActionResult> Profile()
    {
        if (HttpContext.Session.GetString("Token") == null)
            return RedirectToAction("Login", new { returnUrl = "/Account/Profile" });
        var profile = await _api.GetAsync<JObject>("api/auth/profile");
        if (profile == null) return RedirectToAction("Login");
        ViewBag.DanhSachTinh = await _api.GetAsync<List<JObject>>("api/diachi/tinh") ?? new List<JObject>();
        return View(profile);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Profile(string email, string hoTen, string? sdt, int? tinhId, int? phuongXaId, string? soNhaTenDuong)
    {
        var (ok, loi) = await _api.PutAsync("api/auth/profile", new { 
            Email = email, 
            HoTen = hoTen, 
            SDT = sdt, 
            TinhId = tinhId,
            PhuongXaId = phuongXaId,
            SoNhaTenDuong = soNhaTenDuong 
        });
        if (ok)
        {
            TempData["ToastMessage"] = "Cập nhật hồ sơ thành công.";
            HttpContext.Session.SetString("HoTen", hoTen);
        }
        else
        {
            TempData["Loi"] = ParseApiError(loi, "Cập nhật thất bại. Email có thể đã tồn tại.");
        }
        return RedirectToAction("Profile");
    }
}
