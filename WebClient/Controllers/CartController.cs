using WebClient.Models;
using WebClient.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace WebClient.Controllers;
public class CartController : Controller
{
    private readonly CartService _cart;
    private readonly ApiClient _api;
    public CartController(CartService cart, ApiClient api) { _cart = cart; _api = api; }
    public IActionResult Index() => View(_cart.LayGioHang());
    [HttpGet, HttpPost]
    public async Task<IActionResult> Them(int productId, int soLuong = 1)
    {
        if (HttpContext.Session.GetString("Token") == null)
        {
            return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Them", "Cart", new { productId, soLuong }) });
        }
        var p = await _api.GetAsync<JObject>($"api/products/{productId}");
        if (p == null) return NotFound();
        _cart.ThemVaoGio(new CartItem
        {
            ProductId = productId,
            TenSP = p.Value<string>("tenSP")!,
            Gia = p.Value<decimal>("gia"),
            HinhAnh = p.Value<string>("hinhAnh"),
            SoLuong = soLuong
        });
        TempData["ToastMessage"] = $"Đã thêm {soLuong} sản phẩm vào giỏ hàng!";
        return RedirectToAction("Detail", "Products", new { id = productId });
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CapNhat(int productId, int soLuong)
    {
        _cart.CapNhatSoLuong(productId, soLuong);
        return RedirectToAction("Index");
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Xoa(int productId)
    {
        _cart.XoaKhoiGio(productId);
        return RedirectToAction("Index");
    }
    [HttpGet]
    public async Task<IActionResult> PhuongXaTheoTinh(int tinhId)
    {
        var list = await _api.GetAsync<List<JObject>>($"api/diachi/phuong-xa/{tinhId}");
        return Content(JsonConvert.SerializeObject(list ?? new List<JObject>()), "application/json");
    }
    [HttpGet]
    public async Task<IActionResult> Checkout()
    {
        if (HttpContext.Session.GetString("Token") == null)
            return RedirectToAction("Login", "Account", new { returnUrl = "/Cart/Checkout" });
        var cart = _cart.LayGioHang();
        if (cart.Items.Count == 0) return RedirectToAction("Index");

        var promotions = await _api.GetAsync<List<JObject>>("api/promotions");
        if (promotions != null)
        {
            ViewBag.Promotions = promotions.Where(p => 
                p.Value<bool>("trangThai") && 
                p.Value<DateTime>("ngayBatDau") <= DateTime.Now && 
                p.Value<DateTime>("ngayKetThuc") >= DateTime.Now).ToList();
        }

        ViewBag.DanhSachTinh = await _api.GetAsync<List<JObject>>("api/diachi/tinh") ?? new List<JObject>();

        var vm = new CheckoutVM();
        var profile = await _api.GetAsync<JObject>("api/auth/profile");
        if (profile != null)
        {
            vm.SDTNhan = profile.Value<string>("sdt") ?? "";
            vm.SoNhaTenDuongGiao = profile.Value<string>("soNhaTenDuong") ?? profile.Value<string>("diaChi") ?? "";
            vm.TinhId = profile.Value<int?>("tinhId") ?? 0;
            vm.PhuongXaId = profile.Value<int?>("phuongXaId") ?? 0;
        }

        return View(vm);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Checkout(CheckoutVM vm)
    {
        var cart = _cart.LayGioHang();
        if (cart.Items.Count == 0) return RedirectToAction("Index");
        if (!ModelState.IsValid)
        {
            ViewBag.DanhSachTinh = await _api.GetAsync<List<JObject>>("api/diachi/tinh") ?? new List<JObject>();
            return View(vm);
        }
        var body = new
        {
            SoNhaTenDuongGiao = vm.SoNhaTenDuongGiao,
            TinhId = vm.TinhId,
            PhuongXaId = vm.PhuongXaId,
            SDTNhan = vm.SDTNhan,
            XacNhanChinhChu = vm.XacNhanChinhChu,
            GhiChu = vm.GhiChu,
            PhuongThucThanhToan = vm.PhuongThucThanhToan,
            PhuongThucGiaoHang = vm.PhuongThucGiaoHang,
            ChiTiet = cart.Items.Select(i => new { ProductId = i.ProductId, SoLuong = i.SoLuong })
        };
        var (thanhCong, loi, order) = await _api.PostAsync<JObject>("api/orders", body);
        if (!thanhCong)
        {
            ViewBag.Loi = loi ?? "Đặt hàng không thành công, vui lòng thử lại.";
            ViewBag.DanhSachTinh = await _api.GetAsync<List<JObject>>("api/diachi/tinh") ?? new List<JObject>();
            return View(vm);
        }
        _cart.XoaHetGio();
        return RedirectToAction("ThanhCong", new { id = order!.Value<int>("orderId") });
    }
    public IActionResult ThanhCong(int id) => View(id);
}
