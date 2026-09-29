using WebAdmin.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
namespace WebAdmin.Controllers;
public class DashboardController : BaseAdminController
{
    private readonly ApiClient _api;
    public DashboardController(ApiClient api) => _api = api;
    public async Task<IActionResult> Index(string timeRange = "week")
    {
        ViewBag.TimeRange = timeRange;

        
        var list = await _api.GetAsync<JObject>($"api/dashboard/tong-quan?timeRange={timeRange}");
        return View(list ?? JObject.Parse(@"{ ""tongDoanhThu"": 0, ""tongSoDonHang"": 0, ""tongSanPham"": 0, ""sanPhamSapHetHang"": 0, ""topBanChay"": [], ""chartLabels"": [], ""chartData"": [], ""totalBehaviors"": 0, ""recommendedSales"": 0 }"));
    }
    
    [HttpPost]
    public async Task<IActionResult> SeedData()
    {
        var (ok, loi, _) = await _api.PostAsync<object>("api/dashboard/seed-data", null);
        if (!ok) TempData["Loi"] = loi ?? "Lỗi khi gọi API tạo dữ liệu.";
        else TempData["Thanhcong"] = "Đã tạo dữ liệu mẫu thành công!";
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Export()
    {
        var orders = await _api.GetAsync<JArray>("api/orders");
        if (orders == null) return RedirectToAction("Index");
        var csv = new System.Text.StringBuilder();
        csv.AppendLine("Mã Đơn Hàng,Khách Hàng,Tổng Tiền,Trạng Thái,Ngày Đặt,SDT Nhận,Địa Chỉ Giao");
        foreach (var o in orders)
        {
            csv.AppendLine($"{o["orderId"]},{o["user"]?["hoTen"]},{o["tongTien"]},{(string)o["trangThai"]},{o["ngayDat"]},{o["sdtNhan"]},{o["diaChiGiaoDayDu"]}");
        }
        byte[] bytes = System.Text.Encoding.UTF8.GetPreamble().Concat(System.Text.Encoding.UTF8.GetBytes(csv.ToString())).ToArray();
        return File(bytes, "text/csv", "BaoCaoDonHang.csv");
    }

    public async Task<IActionResult> Backup()
    {
        var token = HttpContext.Session.GetString("Token");
        if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Account");

        using var client = new HttpClient(new HttpClientHandler { ServerCertificateCustomValidationCallback = (a, b, c, d) => true });
        client.BaseAddress = new Uri("https://localhost:7001/");
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var res = await client.GetAsync("api/database/backup");
        if (!res.IsSuccessStatusCode)
        {
            TempData["Loi"] = "Sao lưu dữ liệu thất bại.";
            return RedirectToAction("Index");
        }

        var bytes = await res.Content.ReadAsByteArrayAsync();
        var fileName = res.Content.Headers.ContentDisposition?.FileNameStar ?? 
                       res.Content.Headers.ContentDisposition?.FileName ?? 
                       "MedEquip_Backup.bak";
        
        return File(bytes, "application/octet-stream", fileName.Trim('"'));
    }
}