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
            // LoadData(); 
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadData();
        }

        async void LoadData()
        {
            var list = await _service.GetStylists();
            StylistsList.ItemsSource = list;
        }

        async void OnStylistSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is not Stylist selectedStylist)
                return;

            await Navigation.PushAsync(new AppointmentPage(selectedStylist));

            ((CollectionView)sender).SelectedItem = null;
        }
    }
}