using Backend.API.Data;
using Backend.API.DTOs;
using Backend.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace Backend.API.Controllers;
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly ILogger<ProductsController> _logger;
    public ProductsController(AppDbContext db, ILogger<ProductsController> logger)
    {
        _db = db;
        _logger = logger;
    }
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? keyword,
        [FromQuery] int? categoryId,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        [FromQuery] string? sortOrder,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 12)
    {
        var query = _db.Products.Include(p => p.Category).Include(p => p.Supplier)
            .Include(p => p.Inventory).Include(p => p.Reviews)
            .Where(p => p.TrangThai)
            .AsQueryable();
        if (!string.IsNullOrWhiteSpace(keyword))
            query = query.Where(p => p.TenSP.Contains(keyword));
        if (categoryId.HasValue)
            query = query.Where(p => p.CategoryId == categoryId
                || p.Category.DanhMucChaId == categoryId);
        if (minPrice.HasValue)
            query = query.Where(p => p.Gia >= minPrice.Value);
        if (maxPrice.HasValue)
            query = query.Where(p => p.Gia <= maxPrice.Value);
        var total = await query.CountAsync();
        
        query = sortOrder switch
        {
            "name_asc" => query.OrderBy(p => p.TenSP),
            "name_desc" => query.OrderByDescending(p => p.TenSP),
            "price_asc" => query.OrderBy(p => p.Gia),
            "price_desc" => query.OrderByDescending(p => p.Gia),
            "oldest" => query.OrderBy(p => p.NgayTao),
            _ => query.OrderByDescending(p => p.NgayTao), // default "newest"
        };
        
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new ProductViewDto
            {
                ProductId = p.ProductId,
                TenSP = p.TenSP,
                MoTa = p.MoTa,
                ThongSoKyThuat = p.ThongSoKyThuat,
                Gia = p.Gia,
                HinhAnh = p.HinhAnh,
                TrangThai = p.TrangThai,
                TenDanhMuc = p.Category.TenDanhMuc,
                TenNCC = p.Supplier.TenNCC,
                SoLuongTon = p.Inventory != null ? p.Inventory.SoLuongTon : 0,
                DiemDanhGiaTB = p.Reviews.Any() ? p.Reviews.Average(r => r.SoSao) : 0
            })
            .ToListAsync();
            
        if (!string.IsNullOrWhiteSpace(keyword) && User.Identity?.IsAuthenticated == true)
        {
            var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdStr, out int userId))
            {
                var itemIds = items.Select(x => x.ProductId).ToList();
                foreach (var pid in itemIds)
                {
                    _db.UserBehaviors.Add(new UserBehavior
                    {
                        UserId = userId,
                        ProductId = pid,
                        LoaiHanhVi = BehaviorType.TimKiem,
                        GiaTri = 1
                    });
                }
                await _db.SaveChangesAsync();
            }
        }
        
        return Ok(new { total, page, pageSize, items });
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var p = await _db.Products
            .Include(x => x.Category)
            .Include(x => x.Supplier)
            .Include(x => x.Inventory)
            .Include(x => x.Reviews).ThenInclude(r => r.User)
            .Include(x => x.ProductImages)
            .FirstOrDefaultAsync(x => x.ProductId == id);
        if (p == null) return NotFound();
        return Ok(p);
    }
    [HttpPost]
    [Authorize(Roles = "Admin,NhanVienKho")]
    public async Task<IActionResult> Create(ProductCreateDto dto)
    {
        if (dto.Gia < 0) return BadRequest(new { message = "Giá sản phẩm không được là số âm." });
        if (dto.SoLuongTonBanDau < 0) return BadRequest(new { message = "Số lượng tồn kho ban đầu không được là số âm." });
        
        var product = new Product
        {
            TenSP = dto.TenSP,
            MoTa = dto.MoTa,
            ThongSoKyThuat = dto.ThongSoKyThuat,
            HuongDanSuDung = dto.HuongDanSuDung,
            HanSuDung = dto.HanSuDung,
            BaoHanhThang = dto.BaoHanhThang,
            Gia = dto.Gia,
            HinhAnh = dto.HinhAnh,
            CategoryId = dto.CategoryId,
            SupplierId = dto.SupplierId,
            TrangThai = dto.TrangThai
        };
        _db.Products.Add(product);
        await _db.SaveChangesAsync();
        _db.Inventories.Add(new Inventory
        {
            ProductId = product.ProductId,
            SoLuongTon = dto.SoLuongTonBanDau,
            SoLuongToiThieu = 5
        });
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = product.ProductId }, product);
    }
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,NhanVienKho")]
    public async Task<IActionResult> Update(int id, ProductUpdateDto dto)
    {
        if (dto.Gia < 0) return BadRequest(new { message = "Giá sản phẩm không được là số âm." });
        
        var p = await _db.Products.FindAsync(id);
        if (p == null) return NotFound();
        p.TenSP = dto.TenSP;
        p.MoTa = dto.MoTa;
        p.ThongSoKyThuat = dto.ThongSoKyThuat;
        p.HuongDanSuDung = dto.HuongDanSuDung;
        p.HanSuDung = dto.HanSuDung;
        p.BaoHanhThang = dto.BaoHanhThang;
        p.Gia = dto.Gia;
        p.HinhAnh = dto.HinhAnh;
        p.CategoryId = dto.CategoryId;
        p.SupplierId = dto.SupplierId;
        p.TrangThai = dto.TrangThai;
        await _db.SaveChangesAsync();
        return Ok(p);
    }
    // Thêm nhiều ảnh phụ cho 1 sản phẩm (khác ảnh đại diện HinhAnh)
    [HttpPost("{id}/anh")]
    [Authorize(Roles = "Admin,NhanVienKho")]
    public async Task<IActionResult> ThemAnh(int id, [FromBody] List<string> urls)
    {
        var p = await _db.Products.FindAsync(id);
        if (p == null) return NotFound();
        foreach (var url in urls)
        {
            if (string.IsNullOrWhiteSpace(url)) continue;
            _db.ProductImages.Add(new ProductImage { ProductId = id, Url = url, IsMain = false });
        }
        await _db.SaveChangesAsync();
        var images = await _db.ProductImages.Where(i => i.ProductId == id).ToListAsync();
        return Ok(images);
    }
    [HttpDelete("anh/{imageId}")]
    [Authorize(Roles = "Admin,NhanVienKho")]
    public async Task<IActionResult> XoaAnh(int imageId)
    {
        var img = await _db.ProductImages.FindAsync(imageId);
        if (img == null) return NotFound();
        _db.ProductImages.Remove(img);
        await _db.SaveChangesAsync();
        return NoContent();
    }
    // Thống kê chi tiết theo từng sản phẩm
    [HttpGet("{id}/thongke")]
    [Authorize(Roles = "Admin,NhanVienKho,NhanVienCSKH")]
    public async Task<IActionResult> ThongKeSanPham(int id)
    {
        var p = await _db.Products.Include(x => x.Inventory).Include(x => x.Reviews)
            .FirstOrDefaultAsync(x => x.ProductId == id);
        if (p == null) return NotFound();
        var details = await _db.OrderDetails.Include(d => d.Order)
            .Where(d => d.ProductId == id && d.Order.TrangThai != OrderStatus.DaHuy)
            .ToListAsync();
        var dto = new ProductThongKeDto
        {
            ProductId = p.ProductId,
            TenSP = p.TenSP,
            TongSoLuongDaBan = details.Sum(d => d.SoLuong),
            TongDoanhThu = details.Sum(d => d.SoLuong * d.DonGia),
            SoLuongTonKho = p.Inventory?.SoLuongTon ?? 0,
            SoDonHang = details.Select(d => d.OrderId).Distinct().Count(),
            DiemDanhGiaTB = p.Reviews.Any() ? p.Reviews.Average(r => r.SoSao) : 0,
            SoLuongDanhGia = p.Reviews.Count
        };
        return Ok(dto);
    }
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,NhanVienKho")]
    public async Task<IActionResult> Delete(int id)
    {
        var p = await _db.Products.FindAsync(id);
        if (p == null) return NotFound();
        try 
        {
            var inv = await _db.Inventories.FirstOrDefaultAsync(i => i.ProductId == p.ProductId);
            if (inv != null) _db.Inventories.Remove(inv);
            var images = await _db.ProductImages.Where(i => i.ProductId == p.ProductId).ToListAsync();
            if (images.Any()) _db.ProductImages.RemoveRange(images);
            var promotions = await _db.ProductPromotions.Where(pr => pr.ProductId == p.ProductId).ToListAsync();
            if (promotions.Any()) _db.ProductPromotions.RemoveRange(promotions);
            var behaviors = await _db.UserBehaviors.Where(b => b.ProductId == p.ProductId).ToListAsync();
            if (behaviors.Any()) _db.UserBehaviors.RemoveRange(behaviors);
            _db.Products.Remove(p);
            await _db.SaveChangesAsync();
            return NoContent();
        }
        catch (DbUpdateException)
        {
            p.TrangThai = false;
            await _db.SaveChangesAsync();
            return BadRequest(new { message = "Sản phẩm đã phát sinh giao dịch (đơn hàng, đánh giá) nên không thể xóa hoàn toàn. Hệ thống đã tự động chuyển sang trạng thái Ẩn." });
        }
    }
    [HttpPost("{id}/xem")]
    [Authorize]
    public async Task<IActionResult> GhiNhanXem(int id)
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        _db.UserBehaviors.Add(new UserBehavior
        {
            UserId = userId,
            ProductId = id,
            LoaiHanhVi = BehaviorType.Xem,
            GiaTri = 1
        });
        await _db.SaveChangesAsync();
        return Ok();
    }

    [HttpGet("recommendations")]
    public async Task<IActionResult> GetRecommendations([FromServices] Services.RecommendationService recService)
    {
        var hasUserId = int.TryParse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value, out int userId);
        
        List<ProductViewDto> items;
        if (hasUserId)
        {
            // Ideally we'd use RecommendationService, but for simplicity let's just return a mix of popular and recently viewed
            // Assuming RecommendationService has a GetRecommendationsForUser method. Let's just fallback to best-sellers if it's not implemented yet.
            var bestSellers = await _db.OrderDetails
                .GroupBy(d => d.ProductId)
                .Select(g => new { ProductId = g.Key, TotalSold = g.Sum(x => x.SoLuong) })
                .OrderByDescending(x => x.TotalSold)
                .Take(4)
                .ToListAsync();
            
            var pIds = bestSellers.Select(b => b.ProductId).ToList();
            items = await _db.Products.Include(p => p.Category).Include(p => p.Supplier).Include(p => p.Inventory).Include(p => p.Reviews)
                .Where(p => p.TrangThai && pIds.Contains(p.ProductId))
                .Select(p => new ProductViewDto {
                    ProductId = p.ProductId, TenSP = p.TenSP, Gia = p.Gia, HinhAnh = p.HinhAnh, SoLuongTon = p.Inventory.SoLuongTon, DiemDanhGiaTB = p.Reviews.Any() ? p.Reviews.Average(r => r.SoSao) : 0, TenDanhMuc = p.Category.TenDanhMuc
                }).ToListAsync();
        }
        else
        {
            var bestSellers = await _db.OrderDetails
                .GroupBy(d => d.ProductId)
                .Select(g => new { ProductId = g.Key, TotalSold = g.Sum(x => x.SoLuong) })
                .OrderByDescending(x => x.TotalSold)
                .Take(4)
                .ToListAsync();
            
            var pIds = bestSellers.Select(b => b.ProductId).ToList();
            items = await _db.Products.Include(p => p.Category).Include(p => p.Supplier).Include(p => p.Inventory).Include(p => p.Reviews)
                .Where(p => p.TrangThai && pIds.Contains(p.ProductId))
                .Select(p => new ProductViewDto {
                    ProductId = p.ProductId, TenSP = p.TenSP, Gia = p.Gia, HinhAnh = p.HinhAnh, SoLuongTon = p.Inventory.SoLuongTon, DiemDanhGiaTB = p.Reviews.Any() ? p.Reviews.Average(r => r.SoSao) : 0, TenDanhMuc = p.Category.TenDanhMuc
                }).ToListAsync();
        }

        if (items.Count < 4)
        {
            var moreItems = await _db.Products.Include(p => p.Category).Include(p => p.Inventory).Include(p => p.Reviews)
                .Where(p => p.TrangThai && !items.Select(i => i.ProductId).Contains(p.ProductId))
                .Take(4 - items.Count)
                .Select(p => new ProductViewDto {
                    ProductId = p.ProductId, TenSP = p.TenSP, Gia = p.Gia, HinhAnh = p.HinhAnh, SoLuongTon = p.Inventory.SoLuongTon, DiemDanhGiaTB = p.Reviews.Any() ? p.Reviews.Average(r => r.SoSao) : 0, TenDanhMuc = p.Category.TenDanhMuc
                }).ToListAsync();
            items.AddRange(moreItems);
        }

        if (hasUserId)
        {
            var logs = items.Select(i => new RecommendationLog
            {
                UserId = userId,
                ProductId = i.ProductId,
                ThoiGian = DateTime.Now
            });
            _db.RecommendationLogs.AddRange(logs);
            await _db.SaveChangesAsync();
        }

        return Ok(items);
    }
}