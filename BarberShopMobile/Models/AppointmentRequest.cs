namespace BarberShopMobile.Models
{
    public class AppointmentRequest
    {
        public int StylistID { get; set; }
        public string ClientName { get; set; }
        public string Phone { get; set; }
        public DateTime Date { get; set; }
        public int ServiceID { get; set; }
    }
}