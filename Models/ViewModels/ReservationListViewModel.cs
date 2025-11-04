using System.Collections.Generic;

namespace AirBB.Models.ViewModels
{
    public class ReservationListViewModel
    {
        public List<Reservation> Reservations { get; set; } = new List<Reservation>();
        public FilterCriteria Filter { get; set; } = new FilterCriteria();
    }
}