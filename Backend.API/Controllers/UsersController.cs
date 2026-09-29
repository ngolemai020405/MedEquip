using Backend.API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace Backend.API.Controllers;
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,NhanVienCSKH")]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _db;
    public UsersController(AppDbContext db) => _db = db;
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await _db.Users
            .Include(u => u.Role)
            .Select(u => new
            {
                u.UserId,
                u.HoTen,
                u.Email,
                u.SDT,
                u.TrangThai,
                u.NgayTao,
                RoleName = u.Role.RoleName
            })
            .OrderByDescending(u => u.UserId)
            .ToListAsync();
        return Ok(users);
    }
    
    [HttpGet("{id}/details")]
    public async Task<IActionResult> GetDetails(int id)
    {
        var currentUserRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
        var user = await _db.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.UserId == id);
        if (user == null) return NotFound();
        
        // Hierarchy check for viewing details (NhanVien can't view Admin)
        if (currentUserRole != "Admin" && user.Role.RoleName == "Admin")
        {
            return StatusCode(403, new { message = "Bạn không có quyền xem thông tin tài khoản Admin." });
        }
        
        var orders = await _db.Orders
            .Include(o => o.OrderDetails)
            .ThenInclude(od => od.Product)
            .Where(o => o.UserId == id)
            .OrderByDescending(o => o.NgayDat)
            .Select(o => new {
                o.OrderId,
                o.NgayDat,
                o.TongTien,
                o.TrangThai,
                Items = o.OrderDetails.Select(od => new { od.Product.TenSP, od.SoLuong, od.DonGia })
            })
            .ToListAsync();
            
        return Ok(new { user = new { user.UserId, user.HoTen, user.Email, user.SDT, user.Role.RoleName }, orders });
    }

    [HttpPut("{id}/toggle-status")]
    public async Task<IActionResult> ToggleStatus(int id)
    {
        var currentUserRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
        var user = await _db.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.UserId == id);
        if (user == null) return NotFound();
        
        if (user.Role.RoleName == "Admin")
            return StatusCode(403, new { message = "Không thể khóa tài khoản Admin." });
            
        if (currentUserRole != "Admin" && user.Role.RoleName == currentUserRole)
            return StatusCode(403, new { message = "Bạn không thể khóa tài khoản cùng cấp." });

        user.TrangThai = !user.TrangThai;
        await _db.SaveChangesAsync();
        return Ok(new { success = true });
    }
}