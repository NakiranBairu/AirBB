using System;
using Microsoft.EntityFrameworkCore;
using AirBB.Models.ViewModels;
using AirBB.Models.Domain;

namespace AirBB.Models
{
    public class AirBBContext : DbContext
    {
        public AirBBContext(DbContextOptions<AirBBContext> options) : base(options)
        {
        }

        public DbSet<Location> Locations { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Client> Clients { get; set; }
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
        new Location { LocationId = 5, Name = "Los Angeles" },
        new Location { LocationId = 6, Name = "Seattle" },
        new Location { LocationId = 7, Name = "Austin" }
    );

    // Seed Users (including multiple owners for testing)
    modelBuilder.Entity<User>().HasData(
        new User { UserId = 1, Name = "John PropertyOwner", PhoneNumber = "123-456-7890", Email = "john.owner@example.com", SSN = "123-45-6789", UserType = UserType.Owner, DOB = new DateTime(1990, 1, 1) },
        new User { UserId = 2, Name = "Sarah EstateManager", PhoneNumber = "234-567-8901", Email = "sarah.owner@example.com", SSN = "234-56-7890", UserType = UserType.Owner, DOB = new DateTime(1985, 5, 15) },
        new User { UserId = 3, Name = "Admin User", PhoneNumber = "345-678-9012", Email = "admin@airbb.com", SSN = "345-67-8901", UserType = UserType.Admin, DOB = new DateTime(1980, 10, 20) },
        new User { UserId = 4, Name = "Regular Client", PhoneNumber = "456-789-0123", Email = "client@example.com", SSN = "456-78-9012", UserType = UserType.Client, DOB = new DateTime(1995, 3, 8) },
        new User { UserId = 5, Name = "Mike HouseOwner", Email = "mike.owner@example.com", SSN = "567-89-0123", UserType = UserType.Owner, DOB = new DateTime(1988, 7, 12) } // No phone, only email
    );

    // Seed Residences with varied data for testing validations
    modelBuilder.Entity<Residence>().HasData(
        new Residence { ResidenceId = 1, Name = "Chicago Loop Apartment", ResidencePicture = "chicago-apartment.jpg", LocationId = 1, OwnerId = 1, GuestNumber = 4, BedroomNumber = 2, BathroomNumber = 1, BuiltYear = 2010, PricePerNight = 120.00m },
        new Residence { ResidenceId = 2, Name = "New York Studio", ResidencePicture = "ny-studio.jpg", LocationId = 2, OwnerId = 2, GuestNumber = 2, BedroomNumber = 1, BathroomNumber = 1, BuiltYear = 2015, PricePerNight = 150.00m },
        new Residence { ResidenceId = 3, Name = "Boston Townhouse", ResidencePicture = "boston-townhouse.jpg", LocationId = 3, OwnerId = 1, GuestNumber = 6, BedroomNumber = 3, BathroomNumber = 2.5m, BuiltYear = 2005, PricePerNight = 200.00m },
        new Residence { ResidenceId = 4, Name = "Miami Beach House", ResidencePicture = "miami-beach.jpg", LocationId = 4, OwnerId = 5, GuestNumber = 8, BedroomNumber = 4, BathroomNumber = 3, BuiltYear = 1995, PricePerNight = 300.00m },
        new Residence { ResidenceId = 5, Name = "LA Modern Apartment", ResidencePicture = "la-apartment.jpg", LocationId = 5, OwnerId = 2, GuestNumber = 3, BedroomNumber = 1, BathroomNumber = 1.5m, BuiltYear = 2020, PricePerNight = 180.00m }
    );
}
    }
}