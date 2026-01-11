using BarberShopMobile.Services;
using BarberShopMobile.Models;

namespace BarberShopMobile
{
    public partial class MainPage : ContentPage
    {
        StylistService _service;

        public MainPage()
        {
            InitializeComponent();
            _service = new StylistService();
            // LoadData(); <-- SCOATEM ASTA DE AICI
        }

        // Folosim OnAppearing pentru a reîncărca datele când te întorci în pagină
        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadData();
        }

        async void LoadData()
        {
            // Am scos mesajele de eroare ca să fie curat, dar poți să le lași dacă vrei
            var list = await _service.GetStylists();
            StylistsList.ItemsSource = list;
        }

        // --- ACEASTA ESTE METODA NOUĂ PENTRU CLICK ---
        async void OnStylistSelected(object sender, SelectionChangedEventArgs e)
        {
            // Verificăm dacă s-a selectat ceva (uneori evenimentul se declanșează și la deselectare)
            if (e.CurrentSelection.FirstOrDefault() is not Stylist selectedStylist)
                return;

            // Navigăm către pagina de programare și îi trimitem frizerul ales
            await Navigation.PushAsync(new AppointmentPage(selectedStylist));

            // Deselectăm elementul vizual (ca să nu rămână portocaliu când te întorci)
            ((CollectionView)sender).SelectedItem = null;
        }
    }
}