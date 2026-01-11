using System.ComponentModel.DataAnnotations;

namespace BarberShopWeb1.Models
{
    public class Stylist
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string? Specialization { get; set; }

        // --- LINIA NOUĂ ---
        // Aceasta permite stilistului să "știe" ce recenzii are
        public ICollection<Review>? Reviews { get; set; } 
    }
}