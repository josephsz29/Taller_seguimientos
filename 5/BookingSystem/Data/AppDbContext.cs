using BookingSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<BookableService> BookableServices => Set<BookableService>();
        public DbSet<Booking> Bookings => Set<Booking>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // El código de reserva no se puede repetir
            modelBuilder.Entity<Booking>()
                .HasIndex(b => b.Code)
                .IsUnique();

            // No se puede borrar un servicio que ya tiene reservas
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Service)
                .WithMany(s => s.Bookings)
                .HasForeignKey(b => b.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            // Datos iniciales: se insertan al aplicar la migración (Update-Database)
            modelBuilder.Entity<BookableService>().HasData(
                new BookableService
                {
                    Id = 1,
                    Name = "Consulta general",
                    Description = "Valoración inicial de 30 minutos.",
                    DurationMinutes = 30,
                    Price = 50000m
                },
                new BookableService
                {
                    Id = 2,
                    Name = "Asesoría especializada",
                    Description = "Sesión completa con seguimiento de 60 minutos.",
                    DurationMinutes = 60,
                    Price = 90000m
                },
                new BookableService
                {
                    Id = 3,
                    Name = "Revisión rápida",
                    Description = "Control breve de 15 minutos.",
                    DurationMinutes = 15,
                    Price = 25000m
                });
        }
    }
}
