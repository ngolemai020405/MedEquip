using System.ComponentModel.DataAnnotations;
namespace Backend.API.DTOs;

public class OrderCreateDto
{
    [Required(ErrorMessage = "Vui lòng nhập số nhà, tên đường")]
    [MaxLength(200)]
    public string SoNhaTenDuongGiao { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng chọn Tỉnh/Thành phố")]
    public int TinhId { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn Xã/Phường")]
    public int PhuongXaId { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập số điện thoại nhận hàng")]
    [RegularExpression(@"^(0(3[2-9]|5[5689]|7[06789]|8[1-9]|9[0-46-9]))[0-9]{7}$",
        ErrorMessage = "Số điện thoại không đúng định dạng số di động Việt Nam (VD: 0912345678)")]
    public string SDTNhan { get; set; } = null!;

    [Required(ErrorMessage = "Bạn phải xác nhận đây là thông tin chính chủ trước khi đặt hàng")]
    [Range(typeof(bool), "true", "true", ErrorMessage = "Bạn phải xác nhận đây là thông tin chính chủ trước khi đặt hàng")]
    public bool XacNhanChinhChu { get; set; }

    public string? GhiChu { get; set; }
    public string PhuongThucThanhToan { get; set; } = "COD";
    public string PhuongThucGiaoHang { get; set; } = "GiaoHangTieuChuan";

    [MinLength(1, ErrorMessage = "Đơn hàng phải có ít nhất 1 sản phẩm")]
    public List<OrderItemDto> ChiTiet { get; set; } = new();
}
public class OrderItemDto
{
    public int ProductId { get; set; }

    [Range(1, 1000, ErrorMessage = "Số lượng phải từ 1 đến 1000")]
    public int SoLuong { get; set; }
}
public class OrderStatusUpdateDto
{
    public string TrangThai { get; set; } = null!;
    public int? NhanVienXuLyId { get; set; }
}

// Dùng cho dropdown địa chỉ 2 cấp (Tỉnh -> Xã/Phường), theo mô hình hành chính từ 01/07/2025
public class TinhDto { public int TinhId { get; set; } public string TenTinh { get; set; } = null!; }
public class PhuongXaDto { public int PhuongXaId { get; set; } public string TenPhuongXa { get; set; } = null!; }
