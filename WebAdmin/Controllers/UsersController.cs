using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using WebAdmin.Services;
namespace WebAdmin.Controllers;
public class UsersController : BaseAdminController
{
    private readonly ApiClient _api;
    public UsersController(ApiClient api) => _api = api;
    public async Task<IActionResult> Index()
    {
        var users = await _api.GetAsync<JArray>("api/users");
        return View(users);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleStatus(int id)
    {
        var result = await _api.PutAsync($"api/users/{id}/toggle-status", new { });
        if (result.ThanhCong)
            TempData["Thanhcong"] = "Cập nhật trạng thái thành công.";
        else
            TempData["Loi"] = "Bạn không có quyền hoặc có lỗi xảy ra.";
        return RedirectToAction(nameof(Index));
    }
    
    public async Task<IActionResult> Details(int id)
    {
        var result = await _api.GetAsync<JObject>($"api/users/{id}/details");
        if (result == null)
        {
            TempData["Loi"] = "Bạn không có quyền xem thông tin tài khoản này.";
            return RedirectToAction(nameof(Index));
        }
        return View(result);
    }
}
