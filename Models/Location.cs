using System.ComponentModel.DataAnnotations;

namespace AirBB.Models
{
    public class Location
    {
        [Key]
        public int LocationId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        public virtual ICollection<Residence>? Residences { get; set; }
    }
}