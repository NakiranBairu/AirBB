using System.Collections.Generic;

namespace AirBB.Models.ViewModels
{
    public class ResidenceListViewModel
    {
        public List<Residence> Residences { get; set; } = new List<Residence>();
        public List<Location> Locations { get; set; } = new List<Location>();
        public FilterCriteria Filter { get; set; } = new FilterCriteria();
    }
}