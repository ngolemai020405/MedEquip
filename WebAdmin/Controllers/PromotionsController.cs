using WebAdmin.Models;
using WebAdmin.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
namespace WebAdmin.Controllers;
public class PromotionsController : BaseAdminController
{
    private readonly ApiClient _api;
    public PromotionsController(ApiClient api) => _api = api;
    public async Task<IActionResult> Index()
    {
        var list = await _api.GetAsync<List<JObject>>("api/promotions");
        return View(list ?? new List<JObject>());
    }
    public IActionResult Create() => View(new PromotionsVM { NgayBatDau = DateTime.Today, NgayKetThuc = DateTime.Today.AddDays(30) });
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PromotionsVM vm)
    {
        if (vm.NgayBatDau >= vm.NgayKetThuc)
        {
            ViewBag.Loi = "Ngày bắt đầu phải trước ngày kết thúc.";
            return View(vm);
        }
        var (ok, loi, _) = await _api.PostAsync<object>("api/promotions", vm);
        if (!ok) { ViewBag.Loi = loi; return View(vm); }
        TempData["Thanhcong"] = "Thêm khuyến mãi thành công";
        return RedirectToAction("Index");
    }
    public async Task<IActionResult> Edit(int id)
    {
        var p = await _api.GetAsync<PromotionsVM>($"api/promotions/{id}");
        if (p == null) return NotFound();
        return View(p);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, PromotionsVM vm)
    {
        if (vm.NgayBatDau >= vm.NgayKetThuc)
        {
            ViewBag.Loi = "Ngày bắt đầu phải trước ngày kết thúc.";
            return View(vm);
        }
        var (ok, loi) = await _api.PutAsync($"api/promotions/{id}", vm);
        if (!ok) { ViewBag.Loi = loi; return View(vm); }
        TempData["Thanhcong"] = "Cập nhật khuyến mãi thành công";
        return RedirectToAction("Index");
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _api.DeleteAsync($"api/promotions/{id}");
        TempData["Thanhcong"] = "Đã xóa khuyến mãi";
        return RedirectToAction("Index");
    }
}
