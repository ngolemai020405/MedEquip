using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MedEquipMobile.Models;
using MedEquipMobile.Services;

namespace MedEquipMobile.ViewModels;

public partial class OrdersViewModel : BaseViewModel
{
    private readonly ApiService _apiService;

    public ObservableCollection<Order> Orders { get; } = new();

    public OrdersViewModel(ApiService apiService)
    {
        _apiService = apiService;
        Title = "Đơn hàng của tôi";
    }

    [RelayCommand]
    async Task LoadOrdersAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            var orders = await _apiService.GetMyOrdersAsync();
            Orders.Clear();
            foreach (var o in orders)
            {
                Orders.Add(o);
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Lỗi", "Không thể tải danh sách đơn hàng: " + ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
}
