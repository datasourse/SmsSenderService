namespace WebApplication.Settings.Models
{
    public class AppSettings
    {
        public int MaxPhonesQtyPerDay { get; set; }
        public int MaxPhonesQtyPerQuery { get; set; }
        public int MaxMessageLengthLat { get; set; }
        public int MaxMessageLengthNonLat { get; set; }
    }
}