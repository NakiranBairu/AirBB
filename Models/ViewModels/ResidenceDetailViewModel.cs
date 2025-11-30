using System.Collections.Generic;
using AirBB.Models.Domain;
using AirBB.Models.Utilities;

namespace AirBB.Models.ViewModels
{
    public class ResidenceDetailViewModel
    {
        public Residence Residence { get; set; } = new Residence();
        public FilterCriteria Filter { get; set; } = new FilterCriteria();
        public Reservation Reservation { get; set; } = new Reservation();
    }
}