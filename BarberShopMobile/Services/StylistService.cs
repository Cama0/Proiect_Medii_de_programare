using System.Net.Http.Json;
using BarberShopMobile.Models;

namespace BarberShopMobile.Services
{
    public class StylistService
    {
        HttpClient _client;

        public StylistService()
        {
            _client = new HttpClient();
        }

        public async Task<List<Stylist>> GetStylists()
        {
            string port = "5213"; 
            
            string url = $"http://localhost:{port}/api/stylists";

            try
            {
                var response = await _client.GetFromJsonAsync<List<Stylist>>(url);
                return response ?? new List<Stylist>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"EROARE: {ex.Message}");
                return new List<Stylist>();
            }
        }

        public async Task<List<Service>> GetServices()
    {
        string port = "5213";
        string url = $"http://127.0.0.1:{port}/api/services";

        try
        {
            var response = await _client.GetFromJsonAsync<List<Service>>(url);
            return response ?? new List<Service>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Eroare Servicii: {ex.Message}");
            return new List<Service>();
        }
    }
        public async Task<string> SaveAppointment(AppointmentRequest appointment)
{
    string port = "5213";
    string url = $"http://127.0.0.1:{port}/api/appointments";

    try
    {
        var response = await _client.PostAsJsonAsync(url, appointment);
        
        if (response.IsSuccessStatusCode)
        {
            return "Succes";
        }
        else
        {
            string errorMsg = await response.Content.ReadAsStringAsync();
            return errorMsg; 
        }
    }
    catch (Exception ex)
    {
        return $"Eroare conexiune: {ex.Message}";
    }
}
    public async Task<List<AppointmentView>> GetMyAppointments(string phone)
{
    string port = "5213";
    string url = $"http://127.0.0.1:{port}/api/appointments/my-appointments?phone={phone}";

    try
    {
        var response = await _client.GetFromJsonAsync<List<AppointmentView>>(url);
        return response ?? new List<AppointmentView>();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Eroare istoric: {ex.Message}");
        return new List<AppointmentView>();
    }
}

public async Task<bool> DeleteAppointment(int id)
{
    string port = "5213"; // Portul tău
    string url = $"http://127.0.0.1:{port}/api/appointments/{id}";

    try
    {
        var response = await _client.DeleteAsync(url);
        return response.IsSuccessStatusCode;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Eroare ștergere: {ex.Message}");
        return false;
    }
}
    }
}