using WebClient.Models;
using WebClient.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
namespace WebClient.Controllers;
public class OrdersController : Controller
{
    private readonly ApiClient _api;
    public OrdersController(ApiClient api) => _api = api;
    public async Task<IActionResult> Index()
    {
        if (HttpContext.Session.GetString("Token") == null)
            return RedirectToAction("Login", "Account", new { returnUrl = "/Orders" });
        var list = await _api.GetAsync<List<JObject>>("api/orders");
        return View(list ?? new List<JObject>());
    }
    public async Task<IActionResult> Detail(int id)
    {
        var order = await _api.GetAsync<JObject>($"api/orders/{id}");
        if (order == null) return NotFound();
        return View(order);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(int id)
    {
        var (thanhCong, loi, _) = await _api.PostAsync<JObject>($"api/orders/{id}/cancel", null);
        if (thanhCong) TempData["ToastMessage"] = "Đã hủy đơn hàng thành công.";
        else TempData["Loi"] = loi ?? "Không thể hủy đơn hàng này.";
        return RedirectToAction("Detail", new { id });
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ReOrder(int id, [FromServices] CartService cartService)
    {
        var order = await _api.GetAsync<JObject>($"api/orders/{id}");
        if (order == null) return NotFound();
        foreach(var d in order["orderDetails"]!)
        {
            cartService.ThemVaoGio(new CartItem
            {
                ProductId = d.Value<int>("productId"),
                TenSP = d["product"]!.Value<string>("tenSP")!,
                Gia = d.Value<decimal>("donGia"),
                HinhAnh = d["product"]!.Value<string>("hinhAnh"),
                SoLuong = d.Value<int>("soLuong")
            });
        }
        TempData["ToastMessage"] = "Đã thêm các sản phẩm trong đơn cũ vào giỏ hàng.";
        return RedirectToAction("Index", "Cart");
    }
}
