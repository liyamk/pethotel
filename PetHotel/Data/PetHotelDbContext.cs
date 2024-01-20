using Microsoft.EntityFrameworkCore;

namespace PetHotel.Models
{
    public class PetHotelDbContext : DbContext
    {
        public PetHotelDbContext(DbContextOptions<PetHotelDbContext> options)
            : base(options)
        {
        }

        public DbSet<Pet> Pets { get; set; }

        public DbSet<Reservation> Reservations { get; set; }
    }
}
