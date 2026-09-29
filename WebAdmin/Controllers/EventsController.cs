using WebAdmin.Models;
using WebAdmin.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace WebAdmin.Controllers;

public class EventsController : BaseAdminController
{
    private readonly ApiClient _api;

    public EventsController(ApiClient api) => _api = api;

    public async Task<IActionResult> Index()
    {
        var data = await _api.GetAsync<List<JObject>>("api/events");
        return View(data ?? new List<JObject>());
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.Categories = await _api.GetAsync<List<JObject>>("api/categories") ?? new List<JObject>();
        var productsRes = await _api.GetAsync<JObject>("api/products?pageSize=1000");
        ViewBag.Products = productsRes?["items"]?.ToObject<List<JObject>>() ?? new List<JObject>();
        return View(new EventVM());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(EventVM vm)
    {
        if (!ModelState.IsValid)
            return View(vm);
            
        if (vm.HinhAnhUpload != null)
        {
            var filename = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(vm.HinhAnhUpload.FileName);
            var apiPath = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "..", "Backend.API", "wwwroot", "images", "events");
            System.IO.Directory.CreateDirectory(apiPath);
            var filePath = System.IO.Path.Combine(apiPath, filename);
            using (var stream = new System.IO.FileStream(filePath, System.IO.FileMode.Create))
            {
                await vm.HinhAnhUpload.CopyToAsync(stream);
            }
            vm.HinhAnh = "/images/events/" + filename;
        }
        else
        {
            ModelState.AddModelError("HinhAnhUpload", "Vui lòng chọn ảnh sự kiện");
            return View(vm);
        }
        
        var apiPayload = new
        {
            vm.EventId,
            vm.TenSuKien,
            vm.HinhAnh,
            vm.DuongDan,
            vm.NgayBatDau,
            vm.NgayKetThuc,
            vm.TrangThai
        };
        var (ok, loi, result) = await _api.PostAsync<JObject>("api/events", apiPayload);
        if (ok)
        {
            TempData["Thanhcong"] = "Thêm sự kiện thành công!";
            return RedirectToAction("Index");
        }
        
        TempData["Loi"] = loi;
        ViewBag.Categories = await _api.GetAsync<List<JObject>>("api/categories") ?? new List<JObject>();
        var productsRes = await _api.GetAsync<JObject>("api/products?pageSize=1000");
        ViewBag.Products = productsRes?["items"]?.ToObject<List<JObject>>() ?? new List<JObject>();
        return View(vm);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var evt = await _api.GetAsync<JObject>($"api/events/{id}");
        if (evt == null) return NotFound();
        
        var vm = new EventVM
        {
            EventId = (int)evt["eventId"]!,
            TenSuKien = evt["tenSuKien"]?.ToString() ?? "",
            HinhAnh = evt["hinhAnh"]?.ToString(),
            DuongDan = evt["duongDan"]?.ToString(),
            NgayBatDau = (DateTime)evt["ngayBatDau"]!,
            NgayKetThuc = (DateTime)evt["ngayKetThuc"]!,
            TrangThai = (bool)evt["trangThai"]!
        };
        ViewBag.Categories = await _api.GetAsync<List<JObject>>("api/categories") ?? new List<JObject>();
        var productsRes = await _api.GetAsync<JObject>("api/products?pageSize=1000");
        ViewBag.Products = productsRes?["items"]?.ToObject<List<JObject>>() ?? new List<JObject>();
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EventVM vm)
    {
        if (id != vm.EventId) return BadRequest();
        
        if (!ModelState.IsValid)
            return View(vm);
            
        if (vm.HinhAnhUpload != null)
        {
            var filename = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(vm.HinhAnhUpload.FileName);
            var apiPath = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "..", "Backend.API", "wwwroot", "images", "events");
            System.IO.Directory.CreateDirectory(apiPath);
            var filePath = System.IO.Path.Combine(apiPath, filename);
            using (var stream = new System.IO.FileStream(filePath, System.IO.FileMode.Create))
            {
                await vm.HinhAnhUpload.CopyToAsync(stream);
            }
            vm.HinhAnh = "/images/events/" + filename;
        }
        
        var apiPayload = new
        {
            vm.EventId,
            vm.TenSuKien,
            vm.HinhAnh,
            vm.DuongDan,
            vm.NgayBatDau,
            vm.NgayKetThuc,
            vm.TrangThai
        };
        var (ok, loi) = await _api.PutAsync($"api/events/{id}", apiPayload);
        if (ok)
        {
            TempData["Thanhcong"] = "Cập nhật sự kiện thành công!";
            return RedirectToAction("Index");
        }
        
        TempData["Loi"] = loi;
        ViewBag.Categories = await _api.GetAsync<List<JObject>>("api/categories") ?? new List<JObject>();
        var productsRes = await _api.GetAsync<JObject>("api/products?pageSize=1000");
        ViewBag.Products = productsRes?["items"]?.ToObject<List<JObject>>() ?? new List<JObject>();
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var (ok, loi) = await _api.DeleteAsync($"api/events/{id}");
        if (ok)
            return Json(new { success = true });
        return Json(new { success = false, message = loi });
    }
}
