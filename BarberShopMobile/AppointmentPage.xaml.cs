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
        
        GenerateTimeSlots();
    }

    private void GenerateTimeSlots()
    {
        var slots = new List<string>();
        
        TimeSpan startTime = new TimeSpan(9, 0, 0);

        TimeSpan endTime = new TimeSpan(17, 0, 0); 

        while (startTime < endTime)
        {
            slots.Add(startTime.ToString(@"hh\:mm"));
            
            startTime = startTime.Add(TimeSpan.FromMinutes(30));
        }

        PickerTimeSlot.ItemsSource = slots;
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

        if (PickerService.SelectedItem == null)
        {
            await DisplayAlert("Atenție", "Te rog selectează un serviciu.", "OK");
            return;
        }

        if (PickerTimeSlot.SelectedItem == null)
        {
            await DisplayAlert("Atenție", "Te rog alege o oră din listă.", "OK");
            return;
        }

        string selectedTimeStr = (string)PickerTimeSlot.SelectedItem;
        TimeSpan selectedTime = TimeSpan.Parse(selectedTimeStr);

        var selectedService = (Service)PickerService.SelectedItem;

        DateTime fullDate = PickerDate.Date + selectedTime;

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