namespace BarberShopWeb1.Models;

using System.ComponentModel.DataAnnotations;

public class Appointment
{
    public int ID { get; set; }

    [DataType(DataType.Date)]
    public DateTime Date { get; set; }

    public int? MemberID { get; set; }
    public Member? Member { get; set; } 

    public int? StylistID { get; set; }
    public Stylist? Stylist { get; set; } 

    public int? ServiceID { get; set; }
    public Service? Service { get; set; } 
}