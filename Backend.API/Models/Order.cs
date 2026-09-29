namespace Backend.API.Models;
public static class OrderStatus
{
    public const string ChoXacNhan = "ChoXacNhan";
    public const string DaXacNhan = "DaXacNhan";
    public const string DangGiao = "DangGiao";
    public const string DaGiao = "DaGiao";
    public const string DaHuy = "DaHuy";
}
public class Order
{
    public int OrderId { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public DateTime NgayDat { get; set; } = DateTime.Now;
    public string TrangThai { get; set; } = OrderStatus.ChoXacNhan;
    public decimal TongTien { get; set; }

    // Địa chỉ giao hàng chuẩn hoá 2 cấp (chỉ nhập tay số nhà + tên đường, còn lại chọn từ danh mục)
    public string SoNhaTenDuongGiao { get; set; } = null!;
    public int TinhId { get; set; }
    public Tinh Tinh { get; set; } = null!;
    public int PhuongXaId { get; set; }
    public PhuongXa PhuongXa { get; set; } = null!;

    public string SDTNhan { get; set; } = null!;

    // Người mua xác nhận thông tin người nhận là chính chủ (tick trước khi đặt hàng)
    public bool XacNhanChinhChu { get; set; } = false;

    public string? GhiChu { get; set; }
    public string PhuongThucThanhToan { get; set; } = "COD";
    public string PhuongThucGiaoHang { get; set; } = "GiaoHangTieuChuan";
    public bool DaThanhToan { get; set; } = false;
    public int? NhanVienXuLyId { get; set; }
    public User? NhanVienXuLy { get; set; }
    public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public string DiaChiGiaoDayDu =>
        $"{SoNhaTenDuongGiao}, {PhuongXa?.TenPhuongXa}, {Tinh?.TenTinh}";
}
