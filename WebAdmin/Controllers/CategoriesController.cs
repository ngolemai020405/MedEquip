using WebAdmin.Models;
using WebAdmin.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
namespace WebAdmin.Controllers;
public class CategoriesController : BaseAdminController
{
    private readonly ApiClient _api;
    public CategoriesController(ApiClient api) => _api = api;
    public async Task<IActionResult> Index()
    {
        var list = await _api.GetAsync<List<JObject>>("api/categories");
        return View(list ?? new List<JObject>());
    }
    public async Task<IActionResult> Create()
    {
        ViewBag.Categories = await _api.GetAsync<List<JObject>>("api/categories") ?? new List<JObject>();
        return View(new CategoryVM());
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CategoryVM vm)
    {
        var (ok, loi, data) = await _api.PostAsync<JObject>("api/categories", vm);
        if (!ok) 
        { 
            ViewBag.Loi = loi; 
            return View(vm); 
        }
        if (data != null && !string.IsNullOrWhiteSpace(vm.DanhSachDanhMucCon))
        {
            int newParentId = data.Value<int>("categoryId");
            var subs = vm.DanhSachDanhMucCon.Split(',').Select(s => s.Trim()).Where(s => !string.IsNullOrEmpty(s));
            foreach (var sub in subs)
            {
                await _api.PostAsync<object>("api/categories", new CategoryVM { TenDanhMuc = sub, DanhMucChaId = newParentId });
            }
        }
        TempData["Thanhcong"] = "Thêm danh mục thành công";
        return RedirectToAction("Index");
    }
    public async Task<IActionResult> Edit(int id)
    {
        var c = await _api.GetAsync<JObject>($"api/categories/{id}");
        if (c == null) return NotFound();
        ViewBag.Categories = await _api.GetAsync<List<JObject>>("api/categories") ?? new List<JObject>();
        ViewBag.DanhMucCon = c["danhMucCon"]?.ToObject<List<JObject>>();
        var vm = c.ToObject<CategoryVM>();
        return View(vm);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CategoryVM vm)
    {
        var (ok, loi) = await _api.PutAsync($"api/categories/{id}", vm);
        if (!ok) 
        { 
            ViewBag.Loi = loi; 
            ViewBag.Categories = await _api.GetAsync<List<JObject>>("api/categories") ?? new List<JObject>();
            return View(vm); 
        }
        TempData["Thanhcong"] = "Cập nhật danh mục thành công";
        return RedirectToAction("Index");
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var (ok, loi) = await _api.DeleteAsync($"api/categories/{id}");
        if (ok)
            TempData["Thanhcong"] = "Đã xóa danh mục";
        else
            TempData["Loi"] = "Lỗi khi xóa: " + loi;
        return RedirectToAction("Index");
    }
}
