using Newtonsoft.Json;
using WebClient.Models;
namespace WebClient.Services;
public class CartService
{
    private const string SessionKey = "GioHang";
    private readonly IHttpContextAccessor _accessor;
    public CartService(IHttpContextAccessor accessor) => _accessor = accessor;
    private ISession Session => _accessor.HttpContext!.Session;
    public Cart LayGioHang()
    {
        var json = Session.GetString(SessionKey);
        return json == null ? new Cart() : JsonConvert.DeserializeObject<Cart>(json)!;
    }
    private void Luu(Cart cart) => Session.SetString(SessionKey, JsonConvert.SerializeObject(cart));
    public void ThemVaoGio(CartItem item)
    {
        var cart = LayGioHang();
        var existing = cart.Items.FirstOrDefault(i => i.ProductId == item.ProductId);
        if (existing != null) existing.SoLuong += item.SoLuong;
        else cart.Items.Add(item);
        Luu(cart);
    }
    public void CapNhatSoLuong(int productId, int soLuong)
    {
        var cart = LayGioHang();
        var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
        if (item == null) return;
        if (soLuong <= 0) cart.Items.Remove(item);
        else item.SoLuong = soLuong;
        Luu(cart);
    }
    public void XoaKhoiGio(int productId)
    {
        var cart = LayGioHang();
        cart.Items.RemoveAll(i => i.ProductId == productId);
        Luu(cart);
    }
    public void XoaHetGio() => Session.Remove(SessionKey);
}