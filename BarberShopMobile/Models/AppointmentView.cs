namespace BarberShopMobile.Models
{
    public class AppointmentView
    {
        public int ID { get; set; }
        public DateTime Date { get; set; }
        public string StylistName { get; set; }
        public string ServiceName { get; set; }
        
        public string DisplayDate => Date.ToString("dddd, dd MMM - HH:mm");
    }
}