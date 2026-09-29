using Backend.API.Data;
using Backend.API.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace Backend.API.Controllers;

// Địa chỉ hành chính Việt Nam 2 cấp (Tỉnh/Thành phố -> Xã/Phường) áp dụng từ 01/07/2025
[ApiController]
[Route("api/diachi")]
public class DiaChiController : ControllerBase
{
    private readonly AppDbContext _db;
    public DiaChiController(AppDbContext db) { _db = db; }

    // GET api/diachi/tinh
    [HttpGet("tinh")]
    public async Task<IActionResult> GetTinh()
    {
        var list = await _db.Tinhs.OrderBy(t => t.TenTinh)
            .Select(t => new TinhDto { TinhId = t.TinhId, TenTinh = t.TenTinh })
            .ToListAsync();
        return Ok(list);
    }

    // GET api/diachi/phuong-xa/1
    [HttpGet("phuong-xa/{tinhId}")]
    public async Task<IActionResult> GetPhuongXa(int tinhId)
    {
        var list = await _db.PhuongXas.Where(p => p.TinhId == tinhId).OrderBy(p => p.TenPhuongXa)
            .Select(p => new PhuongXaDto { PhuongXaId = p.PhuongXaId, TenPhuongXa = p.TenPhuongXa })
            .ToListAsync();
        return Ok(list);
    }
}
