using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace WebAdmin.Models;
public class LoginVM
{
    public string Email { get; set; } = null!;
    public string MatKhau { get; set; } = null!;
}
public class AuthResponseVM
{
    public string Token { get; set; } = null!;
    public int UserId { get; set; }
    public string HoTen { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string RoleName { get; set; } = null!;
}
public class CategoryVM
{
    public int CategoryId { get; set; }
    public string TenDanhMuc { get; set; } = null!;
    public string? MoTa { get; set; }
    public int? DanhMucChaId { get; set; }
    public string? DanhSachDanhMucCon { get; set; }
}
public class SupplierVM
{
    public int SupplierId { get; set; }
    public string TenNCC { get; set; } = null!;
    public string? DiaChi { get; set; }
    public string? SDT { get; set; }
    public string? Email { get; set; }
    public string? NguoiLienHe { get; set; }
}
public class PromotionsVM
{
    public int PromotionId { get; set; }
    public string TenKM { get; set; } = null!;
    public string? MoTa { get; set; }
    public int PhanTramGiam { get; set; }
    public DateTime NgayBatDau { get; set; }
    public DateTime NgayKetThuc { get; set; }
    public bool TrangThai { get; set; } = true;
}

public class ProductVM
{
    public int ProductId { get; set; }
    
    [Required(ErrorMessage = "Tên sản phẩm là bắt buộc")]
    [MaxLength(200, ErrorMessage = "Tên sản phẩm quá dài")]
    public string TenSP { get; set; } = null!;
    
    public string? MoTa { get; set; }
    public string? ThongSoKyThuat { get; set; }
    public string? HuongDanSuDung { get; set; }

    [DataType(DataType.Date)]
    public DateTime? HanSuDung { get; set; }

    [Range(0, 240, ErrorMessage = "Thời hạn bảo hành (tháng) không hợp lệ")]
    public int BaoHanhThang { get; set; } = 0;

    [Range(0, double.MaxValue, ErrorMessage = "Giá sản phẩm phải hợp lệ")]
    public decimal Gia { get; set; }
    
    public string? HinhAnh { get; set; }
    
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public IFormFile? HinhAnhUpload { get; set; }

    // Nhiều ảnh phụ cho sản phẩm (tối đa nhiều tấm)
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public List<IFormFile>? HinhAnhPhu { get; set; }
    
    // Thêm: Nhiều link ảnh phụ (cách nhau bằng phẩy hoặc xuống dòng)
    public string? HinhAnhPhuLinks { get; set; }
    
    public bool TrangThai { get; set; } = true;
    public int CategoryId { get; set; }
    public int SupplierId { get; set; }
    
    [Range(0, int.MaxValue, ErrorMessage = "Số lượng tồn kho không hợp lệ")]
    public int SoLuongTonBanDau { get; set; }
}