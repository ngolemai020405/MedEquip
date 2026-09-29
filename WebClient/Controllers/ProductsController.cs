using WebClient.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
namespace WebClient.Controllers;
public class ProductsController : Controller
{
    private readonly ApiClient _api;
    public ProductsController(ApiClient api) => _api = api;
    public async Task<IActionResult> Index(string? keyword, int? categoryId, decimal? minPrice, decimal? maxPrice, string? sortOrder, int page = 1)
    {
        var url = $"api/products?page={page}&pageSize=12";
        if (!string.IsNullOrEmpty(keyword)) url += $"&keyword={keyword}";
        if (categoryId.HasValue) url += $"&categoryId={categoryId}";
        if (minPrice.HasValue) url += $"&minPrice={minPrice}";
        if (maxPrice.HasValue) url += $"&maxPrice={maxPrice}";
        if (!string.IsNullOrEmpty(sortOrder)) url += $"&sortOrder={sortOrder}";
        var data = await _api.GetAsync<JObject>(url);
        ViewBag.DanhMuc = await _api.GetAsync<List<JObject>>("api/categories") ?? new List<JObject>();
        ViewBag.Keyword = keyword;
        ViewBag.CategoryId = categoryId;
        ViewBag.MinPrice = minPrice;
        ViewBag.MaxPrice = maxPrice;
        ViewBag.SortOrder = sortOrder;
        return View(data);
    }
    public async Task<IActionResult> Detail(int id)
    {
        var p = await _api.GetAsync<JObject>($"api/products/{id}");
        if (p == null)
        {
            var res = await (new HttpClient(new HttpClientHandler { ServerCertificateCustomValidationCallback = (a, b, c, d) => true }) { BaseAddress = new Uri("https://localhost:7001/") })
                .GetAsync($"api/products/{id}");
            var raw = await res.Content.ReadAsStringAsync();
            return Content($"API Loi: {res.StatusCode} - {raw}");
        }
        bool canReview = false;
        if (HttpContext.Session.GetString("Token") != null)
        {
            await _api.PostAsync<object>($"api/products/{id}/xem", new { });
            var cr = await _api.GetAsync<JObject>($"api/reviews/can-review/{id}");
            if (cr != null && cr["canReview"] != null)
            {
                canReview = (bool)cr["canReview"]!;
            }
        }
        ViewBag.CanReview = canReview;
        var reviews = await _api.GetAsync<List<JObject>>($"api/reviews/san-pham/{id}");
        ViewBag.Reviews = reviews ?? new List<JObject>();
        
        var recommendations = await _api.GetAsync<List<JObject>>("api/products/recommendations");
        ViewBag.Recommendations = recommendations ?? new List<JObject>();
        
        return View(p);
    }
}