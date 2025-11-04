namespace AirBB.Models
{
    public class FilterCriteria
    {
        public int? LocationId { get; set; }
        public DateTime? CheckInDate { get; set; }
        public DateTime? CheckOutDate { get; set; }
        public int? GuestNumber { get; set; } = 1;
    }
}