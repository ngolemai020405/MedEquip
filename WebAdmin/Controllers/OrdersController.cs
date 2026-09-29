using WebAdmin.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
namespace WebAdmin.Controllers;
public class OrdersController : BaseAdminController
{
    private readonly ApiClient _api;
    public OrdersController(ApiClient api) => _api = api;
    public async Task<IActionResult> Index()
    {
        var res = await (new HttpClient(new HttpClientHandler { ServerCertificateCustomValidationCallback = (a,b,c,d) => true }) { BaseAddress = new Uri("https://localhost:7001/") })
            .SendAsync(new HttpRequestMessage(HttpMethod.Get, "api/orders") { Headers = { { "Authorization", "Bearer " + HttpContext.Session.GetString("Token") } } });
        if (!res.IsSuccessStatusCode) ViewBag.LoiRaw = await res.Content.ReadAsStringAsync();
        
        var list = await _api.GetAsync<List<JObject>>("api/orders");
        return View(list ?? new List<JObject>());
    }
    public async Task<IActionResult> Detail(int id)
    {
        var order = await _api.GetAsync<JObject>($"api/orders/{id}");
        if (order == null) return NotFound();
        return View(order);
    }
    public class CapNhatTrangThaiVM { public string TrangThai { get; set; } = null!; }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CapNhatTrangThai(int id, string trangThai)
    {
        await _api.PutAsync($"api/orders/{id}/trangthai", new CapNhatTrangThaiVM { TrangThai = trangThai });
        TempData["Thanhcong"] = "Da cap nhat trang thai don hang";
        return RedirectToAction("Detail", new { id });
    }
}
