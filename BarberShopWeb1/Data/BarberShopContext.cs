using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BarberShopWeb1.Models;

namespace BarberShopWeb1.Data
{
    public class BarberShopContext : IdentityDbContext
    {
        public BarberShopContext(DbContextOptions<BarberShopContext> options)
            : base(options)
        {
        }

        public DbSet<Stylist> Stylist { get; set; } = default!;
        public DbSet<Service> Service { get; set; } = default!;
        public DbSet<Member> Member { get; set; } = default!;
        public DbSet<Appointment> Appointment { get; set; } = default!;
        public DbSet<Review> Review { get; set; } = default!;
    }
}