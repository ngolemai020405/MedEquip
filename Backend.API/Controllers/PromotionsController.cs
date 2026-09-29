using Backend.API.Data;
using Backend.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace Backend.API.Controllers;
[ApiController]
[Route("api/[controller]")]
public class PromotionsController : ControllerBase
{
    private readonly AppDbContext _db;
    public PromotionsController(AppDbContext db) => _db = db;
    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _db.Promotions
        .Include(p => p.ProductPromotions).ThenInclude(pp => pp.Product)
        .ToListAsync());
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var p = await _db.Promotions.FindAsync(id);
        return p == null ? NotFound() : Ok(p);
    }
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(Promotion dto)
    {
        _db.Promotions.Add(dto);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = dto.PromotionId }, dto);
    }
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, Promotion dto)
    {
        var p = await _db.Promotions.FindAsync(id);
        if (p == null) return NotFound();
        p.TenKM = dto.TenKM;
        p.MoTa = dto.MoTa;
        p.PhanTramGiam = dto.PhanTramGiam;
        p.NgayBatDau = dto.NgayBatDau;
        p.NgayKetThuc = dto.NgayKetThuc;
        p.TrangThai = dto.TrangThai;
        await _db.SaveChangesAsync();
        return Ok(p);
    }
    [HttpPost("{id}/gan-san-pham/{productId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GanSanPham(int id, int productId)
    {
        if (await _db.ProductPromotions.AnyAsync(x => x.PromotionId == id && x.ProductId == productId))
            return BadRequest(new { message = "San pham da duoc gan khuyen mai nay" });
        _db.ProductPromotions.Add(new ProductPromotion { PromotionId = id, ProductId = productId });
        await _db.SaveChangesAsync();
        return Ok();
    }
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var p = await _db.Promotions.FindAsync(id);
        if (p == null) return NotFound();
        _db.Promotions.Remove(p);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}