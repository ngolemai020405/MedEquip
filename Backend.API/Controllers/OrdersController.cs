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
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly ILogger<OrdersController> _logger;
    public OrdersController(AppDbContext db, ILogger<OrdersController> logger)
    {
        _db = db;
        _logger = logger;
    }
    private int CurrentUserId => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
    private string CurrentRole => User.FindFirst(ClaimTypes.Role)!.Value;
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var query = _db.Orders.Include(o => o.OrderDetails).ThenInclude(d => d.Product)
            .Include(o => o.User).Include(o => o.Tinh).Include(o => o.PhuongXa).AsQueryable();
        if (CurrentRole == "KhachHang")
            query = query.Where(o => o.UserId == CurrentUserId);
        var list = await query.OrderByDescending(o => o.NgayDat).ToListAsync();
        return Ok(list);
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var o = await _db.Orders.Include(x => x.OrderDetails).ThenInclude(d => d.Product)
            .Include(x => x.User).Include(x => x.Tinh).Include(x => x.PhuongXa)
            .FirstOrDefaultAsync(x => x.OrderId == id);
        if (o == null) return NotFound();
        if (CurrentRole == "KhachHang" && o.UserId != CurrentUserId) return Forbid();
        return Ok(o);
    }
    [HttpPost]
    public async Task<IActionResult> Create(OrderCreateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        if (dto.ChiTiet.Count == 0) return BadRequest(new { message = "Giỏ hàng trống" });
        if (!dto.XacNhanChinhChu)
            return BadRequest(new { message = "Bạn phải xác nhận đây là thông tin chính chủ trước khi đặt hàng" });

        // Ràng buộc thực tế: Xã/Phường phải thuộc đúng Tỉnh/Thành phố đã chọn
        var phuongXa = await _db.PhuongXas.FirstOrDefaultAsync(p => p.PhuongXaId == dto.PhuongXaId);
        if (phuongXa == null || phuongXa.TinhId != dto.TinhId)
            return BadRequest(new { message = "Xã/Phường không thuộc Tỉnh/Thành phố đã chọn" });

        var order = new Order
        {
            UserId = CurrentUserId,
            SoNhaTenDuongGiao = dto.SoNhaTenDuongGiao.Trim(),
            TinhId = dto.TinhId,
            PhuongXaId = dto.PhuongXaId,
            SDTNhan = dto.SDTNhan,
            XacNhanChinhChu = dto.XacNhanChinhChu,
            GhiChu = dto.GhiChu,
            TrangThai = OrderStatus.ChoXacNhan,
            PhuongThucThanhToan = dto.PhuongThucThanhToan,
            PhuongThucGiaoHang = dto.PhuongThucGiaoHang,
            DaThanhToan = dto.PhuongThucThanhToan != "COD"
        };
        decimal tongTien = 0;
        foreach (var item in dto.ChiTiet)
        {
            var product = await _db.Products.FindAsync(item.ProductId);
            if (product == null) continue;
            if (product.HanSuDung.HasValue && product.HanSuDung.Value.Date < DateTime.Now.Date)
                return BadRequest(new { message = $"Sản phẩm {product.TenSP} đã hết hạn sử dụng, không thể đặt hàng" });
            var inv = await _db.Inventories.FirstOrDefaultAsync(i => i.ProductId == item.ProductId);
            if (inv == null || inv.SoLuongTon < item.SoLuong)
                return BadRequest(new { message = $"Sản phẩm {product.TenSP} không đủ số lượng trong kho. (Còn lại: {inv?.SoLuongTon ?? 0})" });

            var detail = new OrderDetail
            {
                ProductId = item.ProductId,
                SoLuong = item.SoLuong,
                DonGia = product.Gia
            };
            tongTien += detail.SoLuong * detail.DonGia;
            order.OrderDetails.Add(detail);
            _db.UserBehaviors.Add(new UserBehavior
            {
                UserId = CurrentUserId,
                ProductId = item.ProductId,
                LoaiHanhVi = BehaviorType.Mua,
                GiaTri = 3
            });
            inv.SoLuongTon -= item.SoLuong;
        }
        order.TongTien = tongTien;
        if (order.PhuongThucGiaoHang == "GiaoHangHoaToc")
            order.TongTien += 50000;
        else
            order.TongTien += 30000;

        _db.Orders.Add(order);
        await _db.SaveChangesAsync();
        _logger.LogInformation($"[OrderCreated] Người dùng {CurrentUserId} đã tạo đơn hàng mới ID: {order.OrderId} - Tổng tiền: {order.TongTien}");
        return CreatedAtAction(nameof(GetById), new { id = order.OrderId }, order);
    }
    [HttpPost("{id}/cancel")]
    public async Task<IActionResult> CancelOrder(int id)
    {
        var order = await _db.Orders.Include(o => o.OrderDetails).FirstOrDefaultAsync(o => o.OrderId == id);
        if (order == null) return NotFound();
        if (order.UserId != CurrentUserId) return Forbid();
        if (order.TrangThai != OrderStatus.ChoXacNhan) return BadRequest(new { message = "Chỉ đơn hàng chờ xác nhận mới được hủy." });
        order.TrangThai = OrderStatus.DaHuy;
        if (order.DaThanhToan)
        {
            order.DaThanhToan = false;
        }
        foreach(var detail in order.OrderDetails)
        {
            var inv = await _db.Inventories.FirstOrDefaultAsync(i => i.ProductId == detail.ProductId);
            if (inv != null) inv.SoLuongTon += detail.SoLuong;
        }
        await _db.SaveChangesAsync();
        _logger.LogWarning($"[OrderCancelled] Đơn hàng ID: {order.OrderId} đã bị hủy bởi người dùng {CurrentUserId}");
        return Ok(order);
    }
    [HttpPut("{id}/trangthai")]
    [Authorize(Roles = "Admin,NhanVienCSKH")]
    public async Task<IActionResult> CapNhatTrangThai(int id, OrderStatusUpdateDto dto)
    {
        var order = await _db.Orders.FindAsync(id);
        if (order == null) return NotFound();
        order.TrangThai = dto.TrangThai;
        if (dto.NhanVienXuLyId.HasValue) order.NhanVienXuLyId = dto.NhanVienXuLyId;
        await _db.SaveChangesAsync();
        return Ok(order);
    }
}