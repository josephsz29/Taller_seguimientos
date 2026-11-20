using Microsoft.EntityFrameworkCore;
using OnlineChronometer.Models;

namespace OnlineChronometer.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Chronometer> Chronometers => Set<Chronometer>();
        public DbSet<ChronometerLap> ChronometerLaps => Set<ChronometerLap>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // No puede haber dos vueltas con el mismo número en un cronómetro.
            // Este índice compuesto también acelera la búsqueda de las vueltas de un cronómetro.
            modelBuilder.Entity<ChronometerLap>()
                .HasIndex(l => new { l.ChronometerId, l.Number })
                .IsUnique();
        }
    }
}
