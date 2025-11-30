using System.Collections.Generic;
using AirBB.Models.Domain;
using AirBB.Models.Utilities;

namespace AirBB.Models.ViewModels
{
    public class ResidenceListViewModel
    {
        public List<Residence> Residences { get; set; } = new List<Residence>();
        public List<Location> Locations { get; set; } = new List<Location>();
        public FilterCriteria Filter { get; set; } = new FilterCriteria();
    }
}