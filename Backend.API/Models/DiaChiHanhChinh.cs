namespace Backend.API.Models;

// Từ 01/07/2025, Việt Nam bỏ cấp Quận/Huyện, chỉ còn 2 cấp: Tỉnh/Thành phố -> Xã/Phường/Đặc khu
public class Tinh
{
    public int TinhId { get; set; }
    public string TenTinh { get; set; } = null!;
    public ICollection<PhuongXa> PhuongXas { get; set; } = new List<PhuongXa>();
}

public class PhuongXa
{
    public int PhuongXaId { get; set; }
    public string TenPhuongXa { get; set; } = null!;
    public int TinhId { get; set; }
    public Tinh Tinh { get; set; } = null!;
}
