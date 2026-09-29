using WebClient.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
namespace WebClient.Controllers;
public class HomeController : Controller
{
    private readonly ApiClient _api;
    public HomeController(ApiClient api) => _api = api;
    public async Task<IActionResult> Index()
    {
        var sanPhamMoi = await _api.GetAsync<JObject>("api/products?page=1&pageSize=8");
        var danhMuc = await _api.GetAsync<List<JObject>>("api/categories");
        var suKien = await _api.GetAsync<List<JObject>>("api/events/active");
        
        var productsByCategory = new List<dynamic>();
        if (danhMuc != null)
        {
            var tasks = danhMuc.Take(5).Select(async c => 
            {
                var pData = await _api.GetAsync<JObject>($"api/products?categoryId={c["categoryId"]}&pageSize=5");
                return new { Id = c["categoryId"]!.ToString(), Name = c["tenDanhMuc"]!.ToString(), Items = pData?["items"]?.ToObject<List<JObject>>() };
            });
            var results = await Task.WhenAll(tasks);
            foreach (var r in results)
            {
                if (r.Items != null && r.Items.Any())
                {
                    productsByCategory.Add(r);
                }
            }
        }
        ViewBag.ProductsByCategory = productsByCategory;

        List<JObject>? goiY = null;
        if (HttpContext.Session.GetString("Token") != null)
        {
            goiY = await _api.GetAsync<List<JObject>>("api/recommendation/top-k?k=6");
        }
        ViewBag.DanhMuc = danhMuc ?? new List<JObject>();
        ViewBag.GoiY = goiY;
        ViewBag.Events = suKien ?? new List<JObject>();
        return View(sanPhamMoi);
    }
    public IActionResult Error() => View();
}