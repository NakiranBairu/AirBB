namespace AirBB.Models.ViewModels
{
    public class HomeViewModel
    {
        public FilterCriteria FilterCriteria { get; set; } = new FilterCriteria();
        public List<Residence> Residences { get; set; } = new List<Residence>();
        public List<Location> Locations { get; set; } = new List<Location>();
    }

    public class ReservationViewModel
    {
        public Reservation Reservation { get; set; } = new Reservation();
        public Residence Residence { get; set; } = new Residence();
        public FilterCriteria Filter { get; set; } = new FilterCriteria();
    }
}