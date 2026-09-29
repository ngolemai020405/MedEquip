using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MedEquipMobile.Models;
using MedEquipMobile.Services;

namespace MedEquipMobile.ViewModels;

public partial class HomeViewModel : BaseViewModel
{
    private readonly ApiService _apiService;
    private readonly CartService _cartService;

    public ObservableCollection<Category> Categories { get; } = new();
    public ObservableCollection<Product> RecommendedProducts { get; } = new();
    public ObservableCollection<Product> LatestProducts { get; } = new();

    public HomeViewModel(ApiService apiService, CartService cartService)
    {
        _apiService = apiService;
        _cartService = cartService;
        Title = "MedEquip";
    }

    [RelayCommand]
    async Task LoadDataAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            
            var cats = await _apiService.GetCategoriesAsync();
            Categories.Clear();
            foreach (var c in cats) Categories.Add(c);

            var latest = await _apiService.GetLatestProductsAsync();
            LatestProducts.Clear();
            foreach (var p in latest) LatestProducts.Add(p);

            int userId = Preferences.Get("UserId", 0);
            if (userId > 0)
            {
                var recommended = await _apiService.GetRecommendedProductsAsync(userId);
                RecommendedProducts.Clear();
                foreach (var p in recommended) RecommendedProducts.Add(p);
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Lỗi", "Không thể tải dữ liệu: " + ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
    
    [RelayCommand]
    async Task AddToCartAsync(Product product)
    {
        if (product == null) return;
        _cartService.AddToCart(product);
        await Shell.Current.DisplayAlert("Thành công", $"Đã thêm {product.Name} vào giỏ hàng", "OK");
    }
}
