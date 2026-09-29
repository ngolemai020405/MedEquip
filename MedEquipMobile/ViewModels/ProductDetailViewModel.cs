using MedEquipMobile.Models;

namespace MedEquipMobile.ViewModels;

[QueryProperty(nameof(ProductId), "ProductId")]
public class ProductDetailViewModel : BaseViewModel
{
    private readonly Services.ApiService _apiService;
    private int _productId;
    private ProductDetail _product;

    public int ProductId
    {
        get => _productId;
        set { _productId = value; LoadProductDetail(); }
    }

    public ProductDetail Product
    {
        get => _product;
        set { _product = value; OnPropertyChanged(); }
    }

    public Command AddToCartCommand { get; }

    public ProductDetailViewModel(Services.ApiService apiService)
    {
        _apiService = apiService;
        Title = "Chi tiết sản phẩm";
        AddToCartCommand = new Command(OnAddToCart);
    }

    private async void LoadProductDetail()
    {
        if (IsBusy) return;
        IsBusy = true;
        
        try
        {
            Product = await _apiService.GetProductDetailAsync(ProductId);
            Title = Product?.Name ?? "Chi tiết sản phẩm";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async void OnAddToCart()
    {
        if (Product == null) return;
        // Logic add to cart here using CartService
        await Application.Current.MainPage.DisplayAlert("Thành công", "Đã thêm vào giỏ hàng", "OK");
    }
}
