using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using MedEquipMobile.Models;

namespace MedEquipMobile.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;
    private string _token;

    public ApiService()
    {
        // Bypass SSL certificate validation for local development on Android emulator
        var handler = new HttpsClientHandlerService().GetPlatformMessageHandler();
        _httpClient = handler != null ? new HttpClient(handler) : new HttpClient();
        
        _httpClient.BaseAddress = new Uri(Constants.ApiUrl + "/");
        _httpClient.Timeout = TimeSpan.FromSeconds(30);
    }

    public void SetToken(string token)
    {
        _token = token;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
    
    public void ClearToken()
    {
        _token = null;
        _httpClient.DefaultRequestHeaders.Authorization = null;
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest req)
    {
        var response = await _httpClient.PostAsJsonAsync("Auth/login", req);
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
            SetToken(result.Token);
            return result;
        }
        return null;
    }

    public async Task<List<Category>> GetCategoriesAsync()
    {
        try 
        {
            return await _httpClient.GetFromJsonAsync<List<Category>>("Categories");
        } 
        catch { return new List<Category>(); }
    }

    public async Task<List<Product>> GetLatestProductsAsync()
    {
        try 
        {
            return await _httpClient.GetFromJsonAsync<List<Product>>("Products?sort=newest");
        }
        catch { return new List<Product>(); }
    }

    public async Task<List<Product>> GetRecommendedProductsAsync(int userId)
    {
        try 
        {
            // Fallback if recommendation endpoint is not available
            var res = await _httpClient.GetAsync($"Recommendation/{userId}");
            if (res.IsSuccessStatusCode)
                return await res.Content.ReadFromJsonAsync<List<Product>>();
            return await GetLatestProductsAsync();
        }
        catch { return new List<Product>(); }
    }

    public async Task<ProductDetail> GetProductDetailAsync(int productId)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<ProductDetail>($"Products/{productId}");
        }
        catch { return null; }
    }

    public async Task<List<Order>> GetMyOrdersAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<Order>>("Orders/my-orders");
        }
        catch { return new List<Order>(); }
    }

    public async Task<bool> PlaceOrderAsync(OrderRequest req)
    {
        try
        {
            var res = await _httpClient.PostAsJsonAsync("Orders", req);
            return res.IsSuccessStatusCode;
        }
        catch { return false; }
    }
}

// Helper to bypass SSL on Android emulator
public class HttpsClientHandlerService
{
    public HttpMessageHandler GetPlatformMessageHandler()
    {
#if ANDROID
        var handler = new Xamarin.Android.Net.AndroidMessageHandler();
        handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
        {
            if (cert != null && cert.Issuer.Equals("CN=localhost"))
                return true;
            return errors == System.Net.Security.SslPolicyErrors.None;
        };
        return handler;
#elif IOS
        var handler = new NSUrlSessionHandler
        {
            TrustOverrideForUrl = (sender, url, trust) => url.StartsWith("https://localhost") || url.StartsWith("https://10.0.2.2")
        };
        return handler;
#else
        return null;
#endif
    }
}
