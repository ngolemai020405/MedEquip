using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using WebAdmin.Services;
namespace WebAdmin.Controllers;
public class ReviewsController : BaseAdminController
{
    private readonly ApiClient _api;
    public ReviewsController(ApiClient api) => _api = api;
    public async Task<IActionResult> Index()
    {
        var reviews = await _api.GetAsync<JArray>("api/reviews");
        return View(reviews);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Duyet(int id)
    {
        var result = await _api.PutAsync($"api/reviews/{id}/duyet", new { });
        if (result.ThanhCong)
            TempData["Thanhcong"] = "Duyệt đánh giá thành công.";
        else
            TempData["Loi"] = "Duyệt thất bại.";
        return RedirectToAction(nameof(Index));
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PhanHoi(int id, string noiDungPhanHoi)
    {
        var result = await _api.PutAsync($"api/reviews/{id}/phan-hoi", noiDungPhanHoi);
        if (result.ThanhCong)
            TempData["Thanhcong"] = "Phản hồi đánh giá thành công.";
        else
            TempData["Loi"] = "Phản hồi thất bại.";
        return RedirectToAction(nameof(Index));
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _api.DeleteAsync($"api/reviews/{id}");
        TempData["Thanhcong"] = "Đã xóa đánh giá thành công.";
        return RedirectToAction(nameof(Index));
    }
}
