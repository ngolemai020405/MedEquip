using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MedEquipMobile.Models;
using MedEquipMobile.Services;

namespace MedEquipMobile.ViewModels;

public partial class CartViewModel : BaseViewModel
{
    private readonly CartService _cartService;
    private readonly ApiService _apiService;

    public ObservableCollection<CartItem> CartItems { get; } = new();

    [ObservableProperty]
    decimal totalAmount;

    [ObservableProperty]
    string name;

    [ObservableProperty]
    string phone;

    [ObservableProperty]
    string address;

    public CartViewModel(CartService cartService, ApiService apiService)
    {
        _cartService = cartService;
        _apiService = apiService;
        Title = "Giỏ hàng";
    }

    [RelayCommand]
    void LoadCart()
    {
        CartItems.Clear();
        foreach (var item in _cartService.GetCartItems())
        {
            CartItems.Add(item);
        }
        UpdateTotal();
    }

    [RelayCommand]
    void RemoveItem(CartItem item)
    {
        if (item == null) return;
        _cartService.RemoveFromCart(item.Product.ProductId);
        CartItems.Remove(item);
        UpdateTotal();
    }

    void UpdateTotal()
    {
        TotalAmount = _cartService.GetTotal();
    }

    [RelayCommand]
    async Task CheckoutAsync()
    {
        if (CartItems.Count == 0)
        {
            await Shell.Current.DisplayAlert("Thông báo", "Giỏ hàng đang trống", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(Address) || string.IsNullOrWhiteSpace(Phone))
        {
            await Shell.Current.DisplayAlert("Lỗi", "Vui lòng nhập số điện thoại và địa chỉ giao hàng", "OK");
            return;
        }

        if (IsBusy) return;

        try
        {
            IsBusy = true;
            var request = new OrderRequest
            {
                PaymentMethod = "COD",
                DiaChiGiaoHang = Address,
                SoDienThoai = Phone,
                Details = CartItems.Select(c => new OrderDetailRequest
                {
                    ProductId = c.Product.ProductId,
                    Quantity = c.Quantity,
                    UnitPrice = c.Product.FinalPrice
                }).ToList()
            };

            bool success = await _apiService.PlaceOrderAsync(request);
            if (success)
            {
                _cartService.ClearCart();
                CartItems.Clear();
                UpdateTotal();
                await Shell.Current.DisplayAlert("Thành công", "Đặt hàng thành công!", "OK");
            }
            else
            {
                await Shell.Current.DisplayAlert("Lỗi", "Đặt hàng thất bại. Vui lòng thử lại sau.", "OK");
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Lỗi", "Lỗi: " + ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
}
