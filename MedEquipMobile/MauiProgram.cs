namespace MedEquipMobile;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
				fonts.AddFont("materialdesignicons-webfont.ttf", "MaterialDesignIcons");
			});

		builder.Services.AddSingleton<MedEquipMobile.Services.ApiService>();
		builder.Services.AddSingleton<MedEquipMobile.Services.CartService>();

		builder.Services.AddTransient<MedEquipMobile.ViewModels.LoginViewModel>();
		builder.Services.AddTransient<MedEquipMobile.Views.LoginPage>();

		builder.Services.AddTransient<MedEquipMobile.ViewModels.HomeViewModel>();
		builder.Services.AddTransient<MedEquipMobile.Views.HomePage>();

		builder.Services.AddTransient<MedEquipMobile.ViewModels.CartViewModel>();
		builder.Services.AddTransient<MedEquipMobile.Views.CartPage>();

		builder.Services.AddTransient<MedEquipMobile.ViewModels.OrdersViewModel>();
		builder.Services.AddTransient<MedEquipMobile.Views.OrdersPage>();

		builder.Services.AddTransient<MedEquipMobile.ViewModels.ProductDetailViewModel>();
		builder.Services.AddTransient<MedEquipMobile.Views.ProductDetailPage>();

		builder.Services.AddTransient<MedEquipMobile.ViewModels.RegisterViewModel>();
		builder.Services.AddTransient<MedEquipMobile.Views.RegisterPage>();

		builder.Services.AddTransient<MedEquipMobile.ViewModels.ProfileViewModel>();
		builder.Services.AddTransient<MedEquipMobile.Views.ProfilePage>();

		builder.Services.AddTransient<MedEquipMobile.ViewModels.OrderDetailViewModel>();
		builder.Services.AddTransient<MedEquipMobile.Views.OrderDetailPage>();

		return builder.Build();
	}
}
