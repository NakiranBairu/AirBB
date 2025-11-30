using System.ComponentModel.DataAnnotations;

namespace AirBB.Models.Domain
{
    public class Location
    {
        [Key]
        public int LocationId { get; set; }

        [Required(ErrorMessage = "Location name is required")]
        [StringLength(100, ErrorMessage = "Location name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        public virtual ICollection<Residence>? Residences { get; set; }
    }
}