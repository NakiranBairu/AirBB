using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;

namespace AirBB.Models.Domain
{
    public class Residence
    {
        [Key]
        public int ResidenceId { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(50, ErrorMessage = "Name cannot exceed 50 characters")]
        [RegularExpression(@"^[a-zA-Z0-9\s]+$", ErrorMessage = "Name can only contain alphanumeric characters and spaces")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Image File Name")]
        public string? ResidencePicture { get; set; }

        [Required(ErrorMessage = "Location is required")]
        [Display(Name = "Location")]
        public int LocationId { get; set; }

        [ForeignKey("LocationId")]
        public virtual Location? Location { get; set; }

        [Required(ErrorMessage = "Owner is required")]
        [Display(Name = "Owner")]
        public int OwnerId { get; set; }

        [ForeignKey("OwnerId")]
        public virtual User? Owner { get; set; }

        [Required(ErrorMessage = "Guest accommodation is required")]
        [Range(1, 20, ErrorMessage = "Guest accommodation must be between 1 and 20")]
        [Display(Name = "Max Guests")]
        public int GuestNumber { get; set; }

        [Required(ErrorMessage = "Number of bedrooms is required")]
        [Range(1, 10, ErrorMessage = "Bedrooms must be between 1 and 10")]
        [Display(Name = "Bedrooms")]
        public int BedroomNumber { get; set; }

        [Required(ErrorMessage = "Number of bathrooms is required")]
        [CustomValidation(typeof(Residence), nameof(ValidateBathrooms))]
        [Display(Name = "Bathrooms")]
        public decimal BathroomNumber { get; set; }

        [Required(ErrorMessage = "Built year is required")]
        [CustomValidation(typeof(Residence), nameof(ValidateBuiltYear))]
        [Display(Name = "Built Year")]
        public int BuiltYear { get; set; }

        [Required(ErrorMessage = "Price per night is required")]
        [Range(0.01, 10000, ErrorMessage = "Price must be greater than 0")]
        [DataType(DataType.Currency)]
        [Display(Name = "Price Per Night")]
        public decimal PricePerNight { get; set; }

        public virtual ICollection<Reservation>? Reservations { get; set; }

        // Custom validation for bathrooms - must be integer or end with .5
        public static ValidationResult? ValidateBathrooms(decimal bathroomNumber, ValidationContext context)
        {
            if (bathroomNumber <= 0 || bathroomNumber > 10)
            {
                return new ValidationResult("Bathrooms must be between 0.5 and 10");
            }

            // Check if it's integer or ends with .5
            if (bathroomNumber % 1 != 0 && bathroomNumber % 1 != 0.5m)
            {
                return new ValidationResult("Bathrooms must be a whole number or end with .5 (e.g., 1, 1.5, 2, 2.5)");
            }

            return ValidationResult.Success;
        }

        // Custom validation for built year - must be past year but not more than 150 years
        public static ValidationResult? ValidateBuiltYear(int builtYear, ValidationContext context)
        {
            var currentYear = DateTime.Now.Year;
            var minYear = currentYear - 150;

            if (builtYear < minYear)
            {
                return new ValidationResult($"Built year cannot be more than 150 years ago. Minimum year is {minYear}");
            }

            if (builtYear > currentYear)
            {
                return new ValidationResult("Built year cannot be in the future");
            }

            return ValidationResult.Success;
        }
    }
}