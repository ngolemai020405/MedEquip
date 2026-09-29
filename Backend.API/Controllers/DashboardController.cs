using Backend.API.Data;
using Backend.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace Backend.API.Controllers;
[ApiController]
[Route("api/[controller]")]
// [Authorize(Roles = "Admin,NhanVienKho,NhanVienCSKH")]
public class DashboardController : ControllerBase
{
    private readonly AppDbContext _db;
    public DashboardController(AppDbContext db) => _db = db;
    [HttpGet("tong-quan")]
    public async Task<IActionResult> TongQuan([FromQuery] string timeRange = "week")
    {
        try
        {
            DateTime startDate = DateTime.Today;
            if (timeRange == "day") startDate = DateTime.Today;
            else if (timeRange == "week") startDate = DateTime.Today.AddDays(-7);
            else if (timeRange == "month") startDate = DateTime.Today.AddMonths(-1);
            else if (timeRange == "year") startDate = DateTime.Today.AddYears(-1);
            var tongDoanhThu = await _db.Orders
                .Where(o => o.TrangThai == OrderStatus.DaGiao && o.NgayDat >= startDate)
                .SumAsync(o => (decimal?)o.TongTien) ?? 0;
            var tongSoDonHang = await _db.Orders.CountAsync(o => o.NgayDat >= startDate);
            var tongSanPham = await _db.Products.CountAsync(p => p.TrangThai);
            var sanPhamSapHetHang = await _db.Inventories.CountAsync(i => i.SoLuongTon <= i.SoLuongToiThieu);
            var topBanChayRaw = await _db.OrderDetails
                .GroupBy(d => d.ProductId)
                .Select(g => new { ProductId = g.Key, SoLuongBan = g.Sum(x => x.SoLuong) })
                .OrderByDescending(x => x.SoLuongBan)
                .Take(5)
                .ToListAsync();
            var productIds = topBanChayRaw.Select(x => x.ProductId).ToList();
            var products = await _db.Products
                .Where(p => productIds.Contains(p.ProductId))
                .ToDictionaryAsync(p => p.ProductId, p => p.TenSP);
            var topBanChay = topBanChayRaw.Select(x => new
            {
                TenSP = products.GetValueOrDefault(x.ProductId, "Sản phẩm không tồn tại"),
                SoLuongBan = x.SoLuongBan
            }).ToList();
            var ordersInPeriod = await _db.Orders
                .Where(o => o.TrangThai == OrderStatus.DaGiao && o.NgayDat >= startDate)
                .ToListAsync();
            List<string> chartLabels = new();
            List<decimal> chartData = new();
            if (timeRange == "day")
            {
                chartLabels.Add("Hôm nay");
                chartData.Add(ordersInPeriod.Sum(o => o.TongTien));
            }
            else if (timeRange == "week")
            {
                for (int i = 6; i >= 0; i--)
                {
                    var d = DateTime.Today.AddDays(-i);
                    chartLabels.Add(d.ToString("dd/MM"));
                    chartData.Add(ordersInPeriod.Where(o => o.NgayDat.Date == d).Sum(o => o.TongTien));
                }
            }
            else if (timeRange == "month")
            {
                for (int i = 4; i >= 0; i--)
                {
                    var startOfWeek = DateTime.Today.AddDays(-((int)DateTime.Today.DayOfWeek)).AddDays(-(i*7));
                    chartLabels.Add("Tuần " + startOfWeek.ToString("dd/MM"));
                    chartData.Add(ordersInPeriod.Where(o => o.NgayDat >= startOfWeek && o.NgayDat < startOfWeek.AddDays(7)).Sum(o => o.TongTien));
                }
            }
            else if (timeRange == "year")
            {
                for (int i = 11; i >= 0; i--)
                {
                    var m = DateTime.Today.AddMonths(-i);
                    chartLabels.Add(m.ToString("MM/yyyy"));
                    chartData.Add(ordersInPeriod.Where(o => o.NgayDat.Month == m.Month && o.NgayDat.Year == m.Year).Sum(o => o.TongTien));
                }
            }
            var totalBehaviors = await _db.UserBehaviors.CountAsync();
            var recommendedSales = await _db.OrderDetails
                .Include(d => d.Order)
                .Where(d => d.Order.TrangThai == OrderStatus.DaGiao &&
                            _db.RecommendationLogs.Any(rl => rl.UserId == d.Order.UserId && rl.ProductId == d.ProductId && rl.ThoiGian <= d.Order.NgayDat))
                .SumAsync(d => (int?)d.SoLuong) ?? 0;
            return Ok(new
            {
                tongDoanhThu,
                tongSoDonHang,
                tongSanPham,
                sanPhamSapHetHang,
                topBanChay,
                chartLabels,
                chartData,
                totalBehaviors,
                recommendedSales
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { loi = ex.ToString() });
        }
    }

    [HttpPost("seed-data")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> SeedData()
    {
        if (!await _db.Categories.AnyAsync()) {
            _db.Categories.AddRange(
                new Category { TenDanhMuc = "Máy đo huyết áp", MoTa = "Các loại máy đo huyết áp" },
                new Category { TenDanhMuc = "Thiết bị trợ thính", MoTa = "Máy trợ thính cao cấp" },
                new Category { TenDanhMuc = "Vật tư y tế", MoTa = "Vật tư tiêu hao" }
            );
            await _db.SaveChangesAsync();
        }

        if (!await _db.Suppliers.AnyAsync()) {
            _db.Suppliers.AddRange(
                new Supplier { TenNCC = "Công ty TBYT Mẫu", DiaChi = "Hà Nội", SDT = "0988888888" }
            );
            await _db.SaveChangesAsync();
        }

        if (!await _db.Products.AnyAsync()) {
            var catId = _db.Categories.First().CategoryId;
            var supId = _db.Suppliers.First().SupplierId;
            var productsToSeed = new List<Product>
            {
                new Product { TenSP = "Máy đo huyết áp Omron", Gia = 850000, CategoryId = catId, SupplierId = supId, TrangThai = true, HinhAnh = "/images/products/omron.jpg" },
                new Product { TenSP = "Máy trợ thính Beurer", Gia = 1200000, CategoryId = catId, SupplierId = supId, TrangThai = true, HinhAnh = "/images/products/beurer.jpg" },
                new Product { TenSP = "Nhiệt kế hồng ngoại", Gia = 650000, CategoryId = catId, SupplierId = supId, TrangThai = true, HinhAnh = "/images/products/nhietke.jpg" },
                new Product { TenSP = "Máy xông khí dung", Gia = 950000, CategoryId = catId, SupplierId = supId, TrangThai = true, HinhAnh = "/images/products/khidung.jpg" },
                new Product { TenSP = "Que test đường huyết", Gia = 350000, CategoryId = catId, SupplierId = supId, TrangThai = true, HinhAnh = "/images/products/quetest.jpg" }
            };
            _db.Products.AddRange(productsToSeed);
            await _db.SaveChangesAsync();

            foreach(var p in productsToSeed)
                _db.Inventories.Add(new Inventory { ProductId = p.ProductId, SoLuongTon = 50, SoLuongToiThieu = 5 });
            await _db.SaveChangesAsync();
        }

        var khRole = await _db.Roles.FirstOrDefaultAsync(r => r.RoleName == "KhachHang");
        if (khRole == null) {
            khRole = new Role { RoleName = "KhachHang" };
            _db.Roles.Add(khRole);
            await _db.SaveChangesAsync();
        }

        var users = await _db.Users.Where(u => u.RoleId == khRole.RoleId).ToListAsync();
        if (!users.Any()) {
            var newUser = new User { HoTen = "Khách Hàng Demo", Email = "khachhang@demo.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456aA@"), RoleId = khRole.RoleId, TrangThai = true, SDT = "0900000000", DiaChi = "Hà Nội" };
            _db.Users.Add(newUser);
            await _db.SaveChangesAsync();
            users.Add(newUser);
        }

        var products = await _db.Products.ToListAsync();
        var random = new Random();
        var orders = new List<Order>();

        var diaChiDemo = await _db.PhuongXas.FirstOrDefaultAsync();
        if (diaChiDemo == null)
            return BadRequest(new { message = "Chưa có dữ liệu Tỉnh/Quận-Huyện/Phường-Xã. Hãy chạy script diachi_seed.sql trước." });

        for (int i = 0; i < 50; i++)
        {
            var user = users[random.Next(users.Count)];
            var ngayDat = DateTime.Today.AddDays(-random.Next(0, 30));
            var order = new Order
            {
                UserId = user.UserId,
                NgayDat = ngayDat,
                TrangThai = OrderStatus.DaGiao,
                SoNhaTenDuongGiao = "123 Đường Demo",
                TinhId = diaChiDemo.TinhId,
                PhuongXaId = diaChiDemo.PhuongXaId,
                SDTNhan = user.SDT ?? "0987654321",
                XacNhanChinhChu = true,
                PhuongThucThanhToan = "COD",
                DaThanhToan = true,
                OrderDetails = new List<OrderDetail>()
            };

            int numItems = random.Next(1, 4);
            decimal tongTien = 0;
            for (int j = 0; j < numItems; j++)
            {
                var product = products[random.Next(products.Count)];
                int soLuong = random.Next(1, 3);
                tongTien += product.Gia * soLuong;
                order.OrderDetails.Add(new OrderDetail { ProductId = product.ProductId, SoLuong = soLuong, DonGia = product.Gia });
            }

            order.TongTien = tongTien;
            orders.Add(order);
        }

        _db.Orders.AddRange(orders);
        await _db.SaveChangesAsync();

        return Ok(new { message = "Tạo dữ liệu mẫu thành công! Đã tạo 5 Sản phẩm và 50 đơn hàng giả lập." });
    }
}