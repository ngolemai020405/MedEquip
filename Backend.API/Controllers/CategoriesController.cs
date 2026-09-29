using Backend.API.Data;
using Backend.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace Backend.API.Controllers;
[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly AppDbContext _db;
    public CategoriesController(AppDbContext db) => _db = db;
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var list = await _db.Categories
            .Include(c => c.DanhMucCon)
            .Where(c => c.DanhMucChaId == null)
            .Select(c => new
            {
                c.CategoryId,
                c.TenDanhMuc,
                c.MoTa,
                ProductCount = _db.Products.Count(p => p.CategoryId == c.CategoryId || p.Category.DanhMucChaId == c.CategoryId),
                DanhMucCon = c.DanhMucCon.Select(x => new { x.CategoryId, x.TenDanhMuc })
            })
            .ToListAsync();
        return Ok(list);
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var c = await _db.Categories
            .Include(x => x.DanhMucCon)
            .FirstOrDefaultAsync(x => x.CategoryId == id);
        if (c == null) return NotFound();
        return Ok(new
        {
            c.CategoryId,
            c.TenDanhMuc,
            c.MoTa,
            c.DanhMucChaId,
            DanhMucCon = c.DanhMucCon.Select(con => new { con.CategoryId, con.TenDanhMuc })
        });
    }
    [HttpPost]
    [Authorize(Roles = "Admin,NhanVienKho")]
    public async Task<IActionResult> Create(Category dto)
    {
        _db.Categories.Add(dto);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = dto.CategoryId }, dto);
    }
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,NhanVienKho")]
    public async Task<IActionResult> Update(int id, Category dto)
    {
        var c = await _db.Categories.FindAsync(id);
        if (c == null) return NotFound();
        c.TenDanhMuc = dto.TenDanhMuc;
        c.MoTa = dto.MoTa;
        c.DanhMucChaId = dto.DanhMucChaId;
        await _db.SaveChangesAsync();
        return Ok(c);
    }
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,NhanVienKho")]
    public async Task<IActionResult> Delete(int id)
    {
        var c = await _db.Categories.FindAsync(id);
        if (c == null) return NotFound();
        if (await _db.Categories.AnyAsync(x => x.DanhMucChaId == id))
        {
            return BadRequest(new { message = "Không thể xóa vì danh mục này đang chứa các danh mục con." });
        }
        if (await _db.Products.AnyAsync(x => x.CategoryId == id))
        {
            return BadRequest(new { message = "Yêu cầu chuyển hết sản phẩm sang danh mục khác trước khi xóa." });
        }
        _db.Categories.Remove(c);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}