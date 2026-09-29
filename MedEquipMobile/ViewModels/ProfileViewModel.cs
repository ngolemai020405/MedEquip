using MedEquipMobile.Models;

namespace MedEquipMobile.ViewModels;

public class ProfileViewModel : BaseViewModel
{
    private readonly Services.ApiService _apiService;
    
    public Command LogoutCommand { get; }
    public Command ChangePasswordCommand { get; }

    public ProfileViewModel(Services.ApiService apiService)
    {
        _apiService = apiService;
        Title = "Tài khoản";
        LogoutCommand = new Command(OnLogout);
        ChangePasswordCommand = new Command(OnChangePassword);
    }

    private async void OnLogout()
    {
        _apiService.ClearToken();
        await Shell.Current.GoToAsync("//LoginPage");
    }
    
    private async void OnChangePassword()
    {
        await Application.Current.MainPage.DisplayAlert("Thông báo", "Chức năng đổi mật khẩu đang cập nhật", "OK");
    }
}
