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
        
        LoadServices();
    }

    private async void LoadServices()
    {
        var services = await _service.GetServices();
        PickerService.ItemsSource = services; 
    }

    private async void OnConfirmClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(EntryName.Text) || string.IsNullOrWhiteSpace(EntryPhone.Text))
        {
            await DisplayAlert("Eroare", "Te rog completează numele și telefonul.", "OK");
            return;
        }

        TimeSpan time = PickerTime.Time;
        if (time.Hours < 9 || time.Hours >= 17)
        {
            await DisplayAlert("Închis", "Programul nostru este 09:00 - 17:00.", "Am înțeles");
            return;
        }

        if (PickerService.SelectedItem == null)
        {
            await DisplayAlert("Atenție", "Te rog selectează un serviciu din listă.", "OK");
            return;
        }

        var selectedService = (Service)PickerService.SelectedItem;

        DateTime fullDate = PickerDate.Date + time;

        var appointment = new AppointmentRequest
        {
            StylistID = _selectedStylist.ID,
            ClientName = EntryName.Text,
            Phone = EntryPhone.Text,
            Date = fullDate,
            
            ServiceID = selectedService.ID 
        };

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