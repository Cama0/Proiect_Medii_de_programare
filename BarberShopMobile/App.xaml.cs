namespace BarberShopMobile;

public partial class App : Application
{
	public App()
	{
    	InitializeComponent();

    	// --- MODIFICAREA ESTE AICI ---
    	// În loc de: MainPage = new MainPage();
    	// Folosim:
    	MainPage = new NavigationPage(new MainPage());
    
    	// Asta îi spune aplicației: "Vreau să încep cu MainPage, 
    	// dar pune-o într-un sistem de navigare ca să pot da PushAsync mai târziu".
	}
}