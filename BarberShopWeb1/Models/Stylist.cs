using System.ComponentModel.DataAnnotations;

namespace BarberShopWeb1.Models
{
    public class Stylist
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string? Specialization { get; set; }
        public ICollection<Review>? Reviews { get; set; } 
    }
}