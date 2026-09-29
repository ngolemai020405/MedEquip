using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MedEquipMobile.Models;
using MedEquipMobile.Services;
using MedEquipMobile.Views;

namespace MedEquipMobile.ViewModels;

public partial class LoginViewModel : BaseViewModel
{
    private readonly ApiService _apiService;

    [ObservableProperty]
    string email;

    [ObservableProperty]
    string password;

    public LoginViewModel(ApiService apiService)
    {
        _apiService = apiService;
        Title = "Đăng nhập";
    }

    [RelayCommand]
    async Task LoginAsync()
    {
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            await Shell.Current.DisplayAlert("Lỗi", "Vui lòng nhập Email và Mật khẩu", "OK");
            return;
        }

        if (IsBusy) return;

        try
        {
            IsBusy = true;
            var response = await _apiService.LoginAsync(new LoginRequest { Email = Email, MatKhau = Password });
            
            if (response != null)
            {
                Preferences.Set("AuthToken", response.Token);
                Preferences.Set("UserId", response.UserId);
                
                // Navigate to main page
                Application.Current.MainPage = new AppShell();
            }
            else
            {
                await Shell.Current.DisplayAlert("Lỗi", "Đăng nhập thất bại. Vui lòng kiểm tra lại thông tin.", "OK");
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Lỗi", $"Không thể kết nối đến máy chủ: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
}
