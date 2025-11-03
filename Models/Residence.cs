using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AirBB.Models
{
    public class Residence
    {
        [Key]
        public int ResidenceId { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        public string? ResidencePicture { get; set; }

        public int LocationId { get; set; }

        [ForeignKey("LocationId")]
        public virtual Location? Location { get; set; }

        [Range(1, 20)]
        public int GuestNumber { get; set; }

        [Range(1, 10)]
        public int BedroomNumber { get; set; }

        [Range(1, 5)]
        public int BathroomNumber { get; set; }

        [DataType(DataType.Currency)]
        public decimal PricePerNight { get; set; }

        public virtual ICollection<Reservation>? Reservations { get; set; }
    }
}