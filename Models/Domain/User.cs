using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AirBB.Models.Domain
{

    public class User : IValidatableObject
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        [Display(Name = "Full Name")]
        public string Name { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Invalid phone number format")]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "SSN is required")]
        [RegularExpression(@"^\d{3}-\d{2}-\d{4}$", ErrorMessage = "SSN must be in format 123-45-6789")]
        public string SSN { get; set; } = string.Empty;

        [Required(ErrorMessage = "User type is required")]
        [Display(Name = "User Type")]
        public UserType UserType { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime DOB { get; set; }

        // Navigation properties
        public virtual ICollection<Residence>? OwnedResidences { get; set; }
        public virtual ICollection<Reservation>? Reservations { get; set; }

        // Custom validation: Either Email or PhoneNumber is required
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(Email) && string.IsNullOrWhiteSpace(PhoneNumber))
            {
                yield return new ValidationResult(
                    "Either Email or Phone Number is required.",
                    new[] { nameof(Email), nameof(PhoneNumber) });
            }
        }
    }

    public enum UserType
    {
        Client,
        Owner, 
        Admin
    }

    // User Type Options for Dropdown
    public static class UserTypeOptions
    {
        public static readonly Dictionary<string, string> All = new()
        {
            { "Client", "Client" },
            { "Owner", "Owner" },
            { "Admin", "Admin" }
        };
    }
}