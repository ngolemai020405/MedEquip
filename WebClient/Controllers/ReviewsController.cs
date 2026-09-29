using Microsoft.AspNetCore.Mvc;
using WebClient.Services;
namespace WebClient.Controllers;
public class ReviewsController : Controller
{
    private readonly ApiClient _api;
    public ReviewsController(ApiClient api) => _api = api;
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(int productId, int soSao, string? noiDung)
    {
        if (HttpContext.Session.GetString("Token") == null)
            return RedirectToAction("Login", "Account", new { returnUrl = $"/Products/Detail/{productId}" });
            
        var (thanhCong, loi, data) = await _api.PostAsync<Newtonsoft.Json.Linq.JObject>("api/reviews", new { ProductId = productId, SoSao = soSao, NoiDung = noiDung });
        
        if (thanhCong)
        {
            var msg = data?.Value<string>("message");
            TempData["ToastMessage"] = !string.IsNullOrEmpty(msg) ? msg : "Đánh giá sản phẩm thành công! Cảm ơn bạn.";
        }
        else
        {
            TempData["Loi"] = loi ?? "Không thể đánh giá sản phẩm này.";
        }
        
        return RedirectToAction("Detail", "Products", new { id = productId });
    }
}
