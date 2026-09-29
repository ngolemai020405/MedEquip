using MedEquipMobile.Models;

namespace MedEquipMobile.ViewModels;

[QueryProperty(nameof(Order), "Order")]
public class OrderDetailViewModel : BaseViewModel
{
    private Order _order;

    public Order Order
    {
        get => _order;
        set { _order = value; OnPropertyChanged(); Title = $"Đơn hàng #{value?.OrderId}"; }
    }

    public OrderDetailViewModel()
    {
        Title = "Chi tiết đơn hàng";
    }
}
