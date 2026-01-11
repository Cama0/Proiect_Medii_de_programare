namespace BarberShopWeb1.Models;

public class Service
{
    public int ID { get; set; }
    public string Name { get; set; } // ex: Tuns scurt
    public decimal Price { get; set; }
    public int DurationMinutes { get; set; }
    public ICollection<Appointment>? Appointments { get; set; }
}