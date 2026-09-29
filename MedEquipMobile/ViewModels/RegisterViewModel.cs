using MedEquipMobile.Models;

namespace MedEquipMobile.ViewModels;

public class RegisterViewModel : BaseViewModel
{
    private readonly Services.ApiService _apiService;
    private string _email;
    private string _password;
    private string _name;

    public string Email { get => _email; set { _email = value; OnPropertyChanged(); } }
    public string Password { get => _password; set { _password = value; OnPropertyChanged(); } }
    public string Name { get => _name; set { _name = value; OnPropertyChanged(); } }

    public Command RegisterCommand { get; }

    public RegisterViewModel(Services.ApiService apiService)
    {
        _apiService = apiService;
        Title = "Đăng ký";
        RegisterCommand = new Command(OnRegister);
    }

    private async void OnRegister()
    {
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password)) return;
        
        IsBusy = true;
        // Call API
        IsBusy = false;
        
        await Application.Current.MainPage.DisplayAlert("Thông báo", "Vui lòng liên hệ Admin để đăng ký.", "OK");
    }
}
