using System;
using System.Collections.Generic;

namespace MedEquipMobile.Models;

public class AuthResponse
{
    public string Token { get; set; }
    public int UserId { get; set; }
    public string HoTen { get; set; }
    public string Email { get; set; }
    public string RoleName { get; set; }
}

public class LoginRequest
{
    public string Email { get; set; }
    public string MatKhau { get; set; }
}

public class RegisterRequest
{
    public string HoTen { get; set; }
    public string Email { get; set; }
    public string MatKhau { get; set; }
    public string SDT { get; set; }
    public string DiaChi { get; set; }
}

public class Category
{
    public int CategoryId { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public string? IconUrl { get; set; }
}

public class Product
{
    public int ProductId { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public decimal? KhuyenMai { get; set; }
    public string? MainImageUrl { get; set; }
    
    // Virtual properties that might be returned by API
    public string CategoryName { get; set; }
    public double AverageRating { get; set; }
    
    public decimal FinalPrice => KhuyenMai.HasValue && KhuyenMai.Value > 0 
        ? Price - (Price * KhuyenMai.Value / 100m) 
        : Price;
}

public class ProductDetail : Product
{
    public List<string> Images { get; set; } = new();
    public List<Review> Reviews { get; set; } = new();
}

public class Review
{
    public int ReviewId { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CustomerName { get; set; }
}

public class CartItem
{
    public Product Product { get; set; }
    public int Quantity { get; set; }
    public decimal Total => Product.FinalPrice * Quantity;
}

public class Order
{
    public int OrderId { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } // ChoXacNhan, DangGiao, HoanThanh...
    public string PaymentMethod { get; set; }
    public string DiaChiGiaoHang { get; set; }
    public string SoDienThoai { get; set; }
    
    public List<OrderDetail> OrderDetails { get; set; } = new();
}

public class OrderDetail
{
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public string MainImageUrl { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

public class OrderRequest
{
    public string PaymentMethod { get; set; }
    public string DiaChiGiaoHang { get; set; }
    public string SoDienThoai { get; set; }
    public List<OrderDetailRequest> Details { get; set; } = new();
}

public class OrderDetailRequest
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
