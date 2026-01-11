namespace BarberShopWeb1.Models;

public class Member
{
    public int ID { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public ICollection<Appointment>? Appointments { get; set; }
}