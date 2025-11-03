using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AirBB.Models
{
    public class Reservation
    {
        [Key]
        public int ReservationId { get; set; }

        [DataType(DataType.Date)]
        public DateTime ReservationStartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime ReservationEndDate { get; set; }

        public int ResidenceId { get; set; }

        [ForeignKey("ResidenceId")]
        public virtual Residence? Residence { get; set; }

        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual Client? Client { get; set; }
    }
}