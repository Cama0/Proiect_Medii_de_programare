using BarberShopMobile.Models;
using BarberShopMobile.Services;

namespace BarberShopMobile;

public partial class AppointmentPage : ContentPage
{
    Stylist _selectedStylist;
    StylistService _service;

    public AppointmentPage(Stylist stylist)
    {
        InitializeComponent();
        _selectedStylist = stylist;
        _service = new StylistService();

        LabelStylistName.Text = stylist.Name;
        
        // 1. NOU: Încărcăm lista de servicii imediat ce intrăm pe pagină
        LoadServices();
    }

    // Această funcție aduce serviciile de pe server și le pune în meniu
    private async void LoadServices()
    {
        var services = await _service.GetServices();
        PickerService.ItemsSource = services; 
    }

    private async void OnConfirmClicked(object sender, EventArgs e)
    {
        // Validare: Nume și Telefon
        if (string.IsNullOrWhiteSpace(EntryName.Text) || string.IsNullOrWhiteSpace(EntryPhone.Text))
        {
            await DisplayAlert("Eroare", "Te rog completează numele și telefonul.", "OK");
            return;
        }

        // Validare: Program 09:00 - 17:00
        TimeSpan time = PickerTime.Time;
        if (time.Hours < 9 || time.Hours >= 17)
        {
            await DisplayAlert("Închis", "Programul nostru este 09:00 - 17:00.", "Am înțeles");
            return;
        }

        // 2. NOU: Validăm dacă a selectat un serviciu din meniu
        if (PickerService.SelectedItem == null)
        {
            await DisplayAlert("Atenție", "Te rog selectează un serviciu din listă.", "OK");
            return;
        }

        // 3. NOU: Extragem serviciul ales ca să îi știm ID-ul
        // (Convertim obiectul generic în tipul 'Service')
        var selectedService = (Service)PickerService.SelectedItem;

        // Construim data completă
        DateTime fullDate = PickerDate.Date + time;

        // Creăm pachetul de date pentru server
        var appointment = new AppointmentRequest
        {
            StylistID = _selectedStylist.ID,
            ClientName = EntryName.Text,
            Phone = EntryPhone.Text,
            Date = fullDate,
            
            // 4. NOU: Trimitem ID-ul serviciului real, nu "1"
            ServiceID = selectedService.ID 
        };

        // Trimitem la server
        string rezultat = await _service.SaveAppointment(appointment);

        if (rezultat == "Succes")
        {
            await DisplayAlert("Gata!", "Te-am programat cu succes.", "Super");
            await Navigation.PopAsync();
        }
        else
        {
            await DisplayAlert("Eroare", rezultat, "Încearcă altă oră");
        }
    }
}