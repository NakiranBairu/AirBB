using System.Collections.Generic;
using AirBB.Models.Domain;
using AirBB.Models.Utilities;

namespace AirBB.Models.ViewModels
{
    public class ReservationListViewModel
    {
        public List<Reservation> Reservations { get; set; } = new List<Reservation>();
        public FilterCriteria Filter { get; set; } = new FilterCriteria();
    }
}