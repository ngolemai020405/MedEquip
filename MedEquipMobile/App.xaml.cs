namespace MedEquipMobile;

public partial class App : Application
{
	public App(MedEquipMobile.Views.LoginPage loginPage)
	{
		InitializeComponent();

		MainPage = loginPage;
	}
}
