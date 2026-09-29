using WebAdmin.Models;
using WebAdmin.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
namespace WebAdmin.Controllers;
public class SuppliersController : BaseAdminController
{
    private readonly ApiClient _api;
    public SuppliersController(ApiClient api) => _api = api;
    public async Task<IActionResult> Index()
    {
        var list = await _api.GetAsync<List<JObject>>("api/suppliers");
        return View(list ?? new List<JObject>());
    }
    public IActionResult Create() => View(new SupplierVM());
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SupplierVM vm)
    {
        var (ok, loi, _) = await _api.PostAsync<object>("api/suppliers", vm);
        if (!ok) { ViewBag.Loi = loi; return View(vm); }
        TempData["Thanhcong"] = "Thêm nhà cung cấp thành công";
        return RedirectToAction("Index");
    }
    public async Task<IActionResult> Edit(int id)
    {
        var s = await _api.GetAsync<SupplierVM>($"api/suppliers/{id}");
        if (s == null) return NotFound();
        return View(s);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, SupplierVM vm)
    {
        var (ok, loi) = await _api.PutAsync($"api/suppliers/{id}", vm);
        if (!ok) { ViewBag.Loi = loi; return View(vm); }
        TempData["Thanhcong"] = "Cập nhật nhà cung cấp thành công";
        return RedirectToAction("Index");
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _api.DeleteAsync($"api/suppliers/{id}");
        TempData["Thanhcong"] = "Đã xóa nhà cung cấp";
        return RedirectToAction("Index");
    }
}
