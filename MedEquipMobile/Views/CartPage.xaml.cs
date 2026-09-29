using MedEquipMobile.ViewModels;

namespace MedEquipMobile.Views;

public partial class CartPage : ContentPage
{
    private readonly CartViewModel _viewModel;

    public CartPage(CartViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _viewModel = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.LoadCartCommand.Execute(null);
    }
}
