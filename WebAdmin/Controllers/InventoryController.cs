using WebAdmin.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
namespace WebAdmin.Controllers;
public class InventoryController : BaseAdminController
{
    private readonly ApiClient _api;
    public InventoryController(ApiClient api) => _api = api;
    public async Task<IActionResult> Index()
    {
        var list = await _api.GetAsync<List<JObject>>("api/inventory");
        return View(list ?? new List<JObject>());
    }
    public class CapNhatKhoVM { public int SoLuongTon { get; set; } public int SoLuongToiThieu { get; set; } }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CapNhat(int productId, int soLuongTon, int soLuongToiThieu)
    {
        await _api.PutAsync($"api/inventory/{productId}", new CapNhatKhoVM { SoLuongTon = soLuongTon, SoLuongToiThieu = soLuongToiThieu });
        TempData["Thanhcong"] = "Da cap nhat ton kho";
        return RedirectToAction("Index");
    }
}
