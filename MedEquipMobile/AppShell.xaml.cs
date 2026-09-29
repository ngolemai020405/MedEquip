namespace MedEquipMobile;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        
        Routing.RegisterRoute(nameof(Views.ProductDetailPage), typeof(Views.ProductDetailPage));
        Routing.RegisterRoute(nameof(Views.OrderDetailPage), typeof(Views.OrderDetailPage));
        Routing.RegisterRoute(nameof(Views.RegisterPage), typeof(Views.RegisterPage));
    }
}
