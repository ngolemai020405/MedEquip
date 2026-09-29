using System.Security.Claims;
using Backend.API.Data;
using Backend.API.DTOs;
using Backend.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace Backend.API.Controllers;
[ApiController]
[Route("api/[controller]")]
public class ReviewsController : ControllerBase
{
    private readonly AppDbContext _db;
    public ReviewsController(AppDbContext db) => _db = db;
    [HttpGet("san-pham/{productId}")]
    public async Task<IActionResult> GetByProduct(int productId)
    {
        var list = await _db.Reviews.Include(r => r.User)
            .Where(r => r.ProductId == productId && r.DaDuyet)
            .OrderByDescending(r => r.NgayDanhGia)
            .Select(r => new { r.ReviewId, r.SoSao, r.NoiDung, r.NgayDanhGia, TenNguoiDanhGia = r.User.HoTen, r.PhanHoi, r.NgayPhanHoi })
            .ToListAsync();
        return Ok(list);
    }
    
    [HttpGet("can-review/{productId}")]
    [Authorize]
    public async Task<IActionResult> CanReview(int productId)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var hasBought = await _db.Orders
            .Include(o => o.OrderDetails)
            .AnyAsync(o => o.UserId == userId && o.TrangThai == OrderStatus.DaGiao && o.OrderDetails.Any(d => d.ProductId == productId));
        return Ok(new { canReview = hasBought });
    }
    [HttpGet]
    [Authorize(Roles = "Admin,NhanVienCSKH")]
    public async Task<IActionResult> GetAll()
    {
        var list = await _db.Reviews
            .Include(r => r.User)
            .Include(r => r.Product)
            .OrderByDescending(r => r.NgayDanhGia)
            .Select(r => new { 
                r.ReviewId, 
                r.SoSao, 
                r.NoiDung, 
                r.NgayDanhGia, 
                r.DaDuyet,
                TenNguoiDanhGia = r.User.HoTen,
                TenSanPham = r.Product.TenSP,
                r.PhanHoi,
                r.NgayPhanHoi
            })
            .ToListAsync();
        return Ok(list);
    }
    [HttpPost]
    [Authorize(Roles = "KhachHang")]
    public async Task<IActionResult> Create(ReviewCreateDto dto)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        
        var hasBought = await _db.Orders
            .Include(o => o.OrderDetails)
            .AnyAsync(o => o.UserId == userId && o.TrangThai == OrderStatus.DaGiao && o.OrderDetails.Any(d => d.ProductId == dto.ProductId));
            
        if (!hasBought)
        {
            return BadRequest(new { message = "Bạn chỉ có thể đánh giá sản phẩm sau khi đã mua và nhận hàng thành công." });
        }

        string[] badWords = { "ngu", "dm", "vl", "cho đe", "dit", "cc" };
        bool containsBadWord = false;
        if (!string.IsNullOrWhiteSpace(dto.NoiDung))
        {
            var contentLower = dto.NoiDung.ToLower();
            containsBadWord = badWords.Any(w => contentLower.Contains(w));
        }

        var review = new Review
        {
            ProductId = dto.ProductId,
            UserId = userId,
            SoSao = dto.SoSao,
            NoiDung = dto.NoiDung,
            DaDuyet = !containsBadWord
        };
        _db.Reviews.Add(review);
        _db.UserBehaviors.Add(new UserBehavior
        {
            UserId = userId,
            ProductId = dto.ProductId,
            LoaiHanhVi = BehaviorType.DanhGia,
            GiaTri = 2
        });
        await _db.SaveChangesAsync();
        await _db.SaveChangesAsync();
        if (containsBadWord) return Ok(new { message = "Đánh giá của bạn chứa từ ngữ nhạy cảm và đang được chờ duyệt." });
        return Ok(review);
    }
    [HttpPut("{id}/duyet")]
    [Authorize(Roles = "Admin,NhanVienCSKH")]
    public async Task<IActionResult> Duyet(int id)
    {
        var r = await _db.Reviews.FindAsync(id);
        if (r == null) return NotFound();
        r.DaDuyet = true;
        await _db.SaveChangesAsync();
        return Ok(r);
    }
    
    [HttpPut("{id}/phan-hoi")]
    [Authorize(Roles = "Admin,NhanVienCSKH")]
    public async Task<IActionResult> Reply(int id, [FromBody] string noiDungPhanHoi)
    {
        var r = await _db.Reviews.FindAsync(id);
        if (r == null) return NotFound();
        r.PhanHoi = noiDungPhanHoi;
        r.NgayPhanHoi = DateTime.Now;
        await _db.SaveChangesAsync();
        return Ok(r);
    }
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,NhanVienCSKH")]
    public async Task<IActionResult> Delete(int id)
    {
        var r = await _db.Reviews.FindAsync(id);
        if (r == null) return NotFound();
        _db.Reviews.Remove(r);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}