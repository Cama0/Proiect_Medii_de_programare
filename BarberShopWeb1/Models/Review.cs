using System.ComponentModel.DataAnnotations;

namespace BarberShopWeb1.Models
{
    public class Review
    {
        public int ID { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        [StringLength(500)]
        public string? Comment { get; set; }

        public int? StylistID { get; set; }
        public Stylist? Stylist { get; set; }

        public int? MemberID { get; set; }
        public Member? Member { get; set; }
    }
}