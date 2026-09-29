using MedEquipMobile.Models;

namespace MedEquipMobile.Services;

public class CartService
{
    // A simple static in-memory cart for demo
    private static List<CartItem> _items = new();

    public List<CartItem> GetCartItems() => _items.ToList();

    public void AddToCart(Product product, int quantity = 1)
    {
        var existing = _items.FirstOrDefault(i => i.Product.ProductId == product.ProductId);
        if (existing != null)
        {
            existing.Quantity += quantity;
        }
        else
        {
            _items.Add(new CartItem { Product = product, Quantity = quantity });
        }
    }

    public void UpdateQuantity(int productId, int quantity)
    {
        var item = _items.FirstOrDefault(i => i.Product.ProductId == productId);
        if (item != null)
        {
            if (quantity <= 0)
                _items.Remove(item);
            else
                item.Quantity = quantity;
        }
    }

    public void RemoveFromCart(int productId)
    {
        var item = _items.FirstOrDefault(i => i.Product.ProductId == productId);
        if (item != null)
            _items.Remove(item);
    }

    public void ClearCart()
    {
        _items.Clear();
    }
    
    public decimal GetTotal()
    {
        return _items.Sum(i => i.Total);
    }
    
    public int GetTotalItems()
    {
        return _items.Sum(i => i.Quantity);
    }
}
