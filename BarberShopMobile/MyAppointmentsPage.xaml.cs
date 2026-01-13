using BarberShopMobile.Services;
using BarberShopMobile.Models;

namespace BarberShopMobile;

public partial class MyAppointmentsPage : ContentPage
{
    StylistService _service;
    string _phoneNumber;

    public MyAppointmentsPage(string phone)
    {
        InitializeComponent();
        _service = new StylistService();
        _phoneNumber = phone;
        
        LoadHistory();
    }

    private async void LoadHistory()
    {
        var list = await _service.GetMyAppointments(_phoneNumber);
        HistoryList.ItemsSource = list;
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
{
    bool confirm = await DisplayAlert("Confirmare", "Ești sigur că vrei să anulezi programarea?", "DA", "NU");
    if (!confirm) return;

    var button = (Button)sender;
    int appointmentId = (int)button.CommandParameter;

    bool success = await _service.DeleteAppointment(appointmentId);

    if (success)
    {
        await DisplayAlert("Succes", "Programarea a fost anulată.", "OK");
        
        LoadHistory();
    }
    else
    {
        await DisplayAlert("Eroare", "Nu am putut șterge programarea.", "OK");
    }
}
}