using WebAdmin.Models;
using WebAdmin.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
namespace WebAdmin.Controllers;
public class ProductsController : BaseAdminController
{
    private readonly ApiClient _api;
    public ProductsController(ApiClient api) => _api = api;
    public async Task<IActionResult> Index(string? keyword, int page = 1)
    {
        var url = $"api/products?page={page}&pageSize=15" + (string.IsNullOrEmpty(keyword) ? "" : $"&keyword={keyword}");
        var data = await _api.GetAsync<JObject>(url);
        ViewBag.Keyword = keyword;
        return View(data);
    }
    private async Task NapDanhSachChonAsync()
    {
        ViewBag.Categories = await _api.GetAsync<List<JObject>>("api/categories") ?? new List<JObject>();
        ViewBag.Suppliers = await _api.GetAsync<List<JObject>>("api/suppliers") ?? new List<JObject>();
    }
    public async Task<IActionResult> Create()
    {
        await NapDanhSachChonAsync();
        return View(new ProductVM());
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductVM vm)
    {
        if (!ModelState.IsValid)
        {
            await NapDanhSachChonAsync();
            return View(vm);
        }
        
        if (vm.HinhAnhUpload != null)
        {
            var filename = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(vm.HinhAnhUpload.FileName);
            var apiPath = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "..", "Backend.API", "wwwroot", "images", "products");
            System.IO.Directory.CreateDirectory(apiPath);
            var filePath = System.IO.Path.Combine(apiPath, filename);
            using (var stream = new System.IO.FileStream(filePath, System.IO.FileMode.Create))
            {
                await vm.HinhAnhUpload.CopyToAsync(stream);
            }
            vm.HinhAnh = "/images/products/" + filename;
        }
        
        var (ok, loi, result) = await _api.PostAsync<JObject>("api/products", vm);
        if (!ok) { ViewBag.Loi = loi; await NapDanhSachChonAsync(); return View(vm); }
        var newId = result?.Value<int>("productId") ?? 0;
        if (newId > 0)
        {
            await LuuAnhPhuAsync(newId, vm.HinhAnhPhu);
            await LuuAnhPhuLinksAsync(newId, vm.HinhAnhPhuLinks);
        }
        TempData["Thanhcong"] = "Thêm sản phẩm thành công";
        return RedirectToAction("Index");
    }
    private async Task LuuAnhPhuAsync(int productId, List<IFormFile>? files)
    {
        if (files == null || files.Count == 0) return;
        var urls = new List<string>();
        var apiPath = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "..", "Backend.API", "wwwroot", "images", "products");
        System.IO.Directory.CreateDirectory(apiPath);
        foreach (var file in files)
        {
            if (file.Length == 0) continue;
            var filename = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(file.FileName);
            var filePath = System.IO.Path.Combine(apiPath, filename);
            using (var stream = new System.IO.FileStream(filePath, System.IO.FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            urls.Add("/images/products/" + filename);
        }
        if (urls.Count > 0)
            await _api.PostAsync<object>($"api/products/{productId}/anh", urls);
    }
    private async Task LuuAnhPhuLinksAsync(int productId, string? links)
    {
        if (string.IsNullOrWhiteSpace(links)) return;
        var urls = links.Split(new[] { ',', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(u => u.Trim())
                        .Where(u => !string.IsNullOrEmpty(u))
                        .ToList();
        if (urls.Count > 0)
            await _api.PostAsync<object>($"api/products/{productId}/anh", urls);
    }
    public async Task<IActionResult> Edit(int id)
    {
        var p = await _api.GetAsync<JObject>($"api/products/{id}");
        if (p == null) return NotFound();
        await NapDanhSachChonAsync();
        var vm = new ProductVM
        {
            ProductId = p.Value<int>("productId"),
            TenSP = p.Value<string>("tenSP") ?? "",
            MoTa = p.Value<string>("moTa"),
            ThongSoKyThuat = p.Value<string>("thongSoKyThuat"),
            HuongDanSuDung = p.Value<string>("huongDanSuDung"),
            HanSuDung = p["hanSuDung"] != null ? p.Value<DateTime?>("hanSuDung") : null,
            BaoHanhThang = p.Value<int?>("baoHanhThang") ?? 0,
            Gia = p.Value<decimal>("gia"),
            HinhAnh = p.Value<string>("hinhAnh"),
            TrangThai = p.Value<bool>("trangThai"),
            CategoryId = p.Value<int>("categoryId"),
            SupplierId = p.Value<int>("supplierId")
        };
        ViewBag.ExistingImages = p["productImages"];
        return View(vm);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ProductVM vm)
    {
        if (!ModelState.IsValid)
        {
            await NapDanhSachChonAsync();
            return View(vm);
        }
        
        if (vm.HinhAnhUpload != null)
        {
            var filename = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(vm.HinhAnhUpload.FileName);
            var apiPath = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "..", "Backend.API", "wwwroot", "images", "products");
            System.IO.Directory.CreateDirectory(apiPath);
            var filePath = System.IO.Path.Combine(apiPath, filename);
            using (var stream = new System.IO.FileStream(filePath, System.IO.FileMode.Create))
            {
                await vm.HinhAnhUpload.CopyToAsync(stream);
            }
            vm.HinhAnh = "/images/products/" + filename;
        }

        var (ok, loi) = await _api.PutAsync($"api/products/{id}", vm);
        if (!ok) { ViewBag.Loi = loi; await NapDanhSachChonAsync(); return View(vm); }
        await LuuAnhPhuAsync(id, vm.HinhAnhPhu);
        await LuuAnhPhuLinksAsync(id, vm.HinhAnhPhuLinks);
        TempData["Thanhcong"] = "Cập nhật sản phẩm thành công";
        return RedirectToAction("Index");
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _api.DeleteAsync($"api/products/{id}");
        TempData["Thanhcong"] = "Da ngung kinh doanh san pham";
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> DeleteImage(int imageId)
    {
        var (ok, loi) = await _api.DeleteAsync($"api/products/anh/{imageId}");
        return Json(new { success = ok });
    }

    public async Task<IActionResult> ThongKe(int id)
    {
        var product = await _api.GetAsync<JObject>($"api/products/{id}");
        if (product == null) return NotFound();

        var thongKe = await _api.GetAsync<JObject>($"api/products/{id}/thongke");
        
        ViewBag.Product = product;
        return View(thongKe);
    }
}
