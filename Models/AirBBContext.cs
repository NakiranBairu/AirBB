using Microsoft.EntityFrameworkCore;
using AirBB.Models.ViewModels;

namespace AirBB.Models
{
    public class AirBBContext : DbContext
    {
        public AirBBContext(DbContextOptions<AirBBContext> options) : base(options) { }

        public DbSet<Client> Clients { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Residence> Residences { get; set; }
        public DbSet<Reservation> Reservations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Seed Locations
            modelBuilder.Entity<Location>().HasData(
                new Location { LocationId = 1, Name = "Chicago" },
                new Location { LocationId = 2, Name = "New York" },
                new Location { LocationId = 3, Name = "Boston" },
                new Location { LocationId = 4, Name = "Miami" },
                new Location { LocationId = 5, Name = "Los Angeles" }
            );

            // Seed Residences
            modelBuilder.Entity<Residence>().HasData(
                new Residence { ResidenceId = 1, Name = "Chicago Loop Apartment", ResidencePicture = "chicago-apartment.jpg", LocationId = 1, GuestNumber = 4, BedroomNumber = 2, BathroomNumber = 1, PricePerNight = 120.00m },
                new Residence { ResidenceId = 2, Name = "New York Studio", ResidencePicture = "ny-studio.jpg", LocationId = 2, GuestNumber = 2, BedroomNumber = 1, BathroomNumber = 1, PricePerNight = 150.00m },
                new Residence { ResidenceId = 3, Name = "Boston Townhouse", ResidencePicture = "boston-townhouse.jpg", LocationId = 3, GuestNumber = 6, BedroomNumber = 3, BathroomNumber = 2, PricePerNight = 200.00m },
                new Residence { ResidenceId = 4, Name = "Miami Beach House", ResidencePicture = "miami-beach.jpg", LocationId = 4, GuestNumber = 8, BedroomNumber = 4, BathroomNumber = 3, PricePerNight = 300.00m },
                new Residence { ResidenceId = 5, Name = "LA Modern Apartment", ResidencePicture = "la-apartment.jpg", LocationId = 5, GuestNumber = 3, BedroomNumber = 1, BathroomNumber = 1, PricePerNight = 180.00m }
            );

            // Seed a sample client
            modelBuilder.Entity<Client>().HasData(
                new Client { UserId = 1, Name = "John Doe", PhoneNumber = "123-456-7890", Email = "john@example.com", DOB = new DateTime(1990, 1, 1) }
            );
        }
    }
}