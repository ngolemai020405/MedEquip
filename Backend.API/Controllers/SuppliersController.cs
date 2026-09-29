using Backend.API.Data;
using Backend.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace Backend.API.Controllers;
[ApiController]
[Route("api/[controller]")]
public class SuppliersController : ControllerBase
{
    private readonly AppDbContext _db;
    public SuppliersController(AppDbContext db) => _db = db;
    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _db.Suppliers.ToListAsync());
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var s = await _db.Suppliers.FindAsync(id);
        return s == null ? NotFound() : Ok(s);
    }
    [HttpPost]
    [Authorize(Roles = "Admin,NhanVienKho")]
    public async Task<IActionResult> Create(Supplier dto)
    {
        _db.Suppliers.Add(dto);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = dto.SupplierId }, dto);
    }
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,NhanVienKho")]
    public async Task<IActionResult> Update(int id, Supplier dto)
    {
        var s = await _db.Suppliers.FindAsync(id);
        if (s == null) return NotFound();
        s.TenNCC = dto.TenNCC;
        s.DiaChi = dto.DiaChi;
        s.SDT = dto.SDT;
        s.Email = dto.Email;
        s.NguoiLienHe = dto.NguoiLienHe;
        await _db.SaveChangesAsync();
        return Ok(s);
    }
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,NhanVienKho")]
    public async Task<IActionResult> Delete(int id)
    {
        var s = await _db.Suppliers.FindAsync(id);
        if (s == null) return NotFound();
        _db.Suppliers.Remove(s);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}