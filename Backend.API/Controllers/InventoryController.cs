using Backend.API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace Backend.API.Controllers;
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,NhanVienKho")]
public class InventoryController : ControllerBase
{
    private readonly AppDbContext _db;
    public InventoryController(AppDbContext db) => _db = db;
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var list = await _db.Inventories
            .Include(i => i.Product)
            .Select(i => new
            {
                i.InventoryId,
                i.ProductId,
                TenSP = i.Product.TenSP,
                i.SoLuongTon,
                i.SoLuongToiThieu,
                i.NgayCapNhat,
                CanhBaoSapHet = i.SoLuongTon <= i.SoLuongToiThieu
            })
            .ToListAsync();
        return Ok(list);
    }
    public class CapNhatKhoDto { public int SoLuongTon { get; set; } public int SoLuongToiThieu { get; set; } }
    [HttpPut("{productId}")]
    public async Task<IActionResult> CapNhat(int productId, CapNhatKhoDto dto)
    {
        if (dto.SoLuongTon < 0 || dto.SoLuongToiThieu < 0) return BadRequest(new { message = "Số lượng không được là số âm." });
        var inv = await _db.Inventories.FirstOrDefaultAsync(i => i.ProductId == productId);
        if (inv == null) return NotFound();
        inv.SoLuongTon = dto.SoLuongTon;
        inv.SoLuongToiThieu = dto.SoLuongToiThieu;
        inv.NgayCapNhat = DateTime.Now;
        await _db.SaveChangesAsync();
        return Ok(inv);
    }
}