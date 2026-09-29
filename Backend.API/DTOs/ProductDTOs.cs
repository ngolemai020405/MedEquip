using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
namespace Backend.API.DTOs;
public class ProductCreateDto
{
    [Required(ErrorMessage = "Tên sản phẩm là bắt buộc")]
    [MaxLength(200, ErrorMessage = "Tên sản phẩm quá dài")]
    public string TenSP { get; set; } = null!;
    public string? MoTa { get; set; }
    public string? ThongSoKyThuat { get; set; }
    public string? HuongDanSuDung { get; set; }
    public DateTime? HanSuDung { get; set; }

    [Range(0, 240, ErrorMessage = "Thời hạn bảo hành (tháng) không hợp lệ")]
    public int BaoHanhThang { get; set; } = 0;

    [Range(0, 1000000000, ErrorMessage = "Giá sản phẩm phải hợp lệ")]
    public decimal Gia { get; set; }
    public string? HinhAnh { get; set; }
    public int CategoryId { get; set; }
    public int SupplierId { get; set; }
    public bool TrangThai { get; set; } = true;

    [Range(0, 100000, ErrorMessage = "Số lượng tồn kho không hợp lệ")]
    public int SoLuongTonBanDau { get; set; } = 0;
}
public class ProductUpdateDto
{
    public string TenSP { get; set; } = null!;
    public string? MoTa { get; set; }
    public string? ThongSoKyThuat { get; set; }
    public string? HuongDanSuDung { get; set; }
    public DateTime? HanSuDung { get; set; }
    public int BaoHanhThang { get; set; } = 0;
    public decimal Gia { get; set; }
    public string? HinhAnh { get; set; }
    public int CategoryId { get; set; }
    public int SupplierId { get; set; }
    public bool TrangThai { get; set; }
}
public class ProductViewDto
{
    public int ProductId { get; set; }
    public string TenSP { get; set; } = null!;
    public string? MoTa { get; set; }
    public string? ThongSoKyThuat { get; set; }
    public string? HuongDanSuDung { get; set; }
    public DateTime? HanSuDung { get; set; }
    public int BaoHanhThang { get; set; }
    public decimal Gia { get; set; }
    public string? HinhAnh { get; set; }
    public bool TrangThai { get; set; }
    public string TenDanhMuc { get; set; } = null!;
    public string TenNCC { get; set; } = null!;
    public int SoLuongTon { get; set; }
    public double DiemDanhGiaTB { get; set; }
}

// Thống kê chi tiết theo từng sản phẩm (cho trang admin)
public class ProductThongKeDto
{
    public int ProductId { get; set; }
    public string TenSP { get; set; } = null!;
    public int TongSoLuongDaBan { get; set; }
    public decimal TongDoanhThu { get; set; }
    public int SoLuongTonKho { get; set; }
    public int SoDonHang { get; set; }
    public double DiemDanhGiaTB { get; set; }
    public int SoLuongDanhGia { get; set; }
}
