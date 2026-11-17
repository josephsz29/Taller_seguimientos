using EventCalendar.Models;
using Microsoft.EntityFrameworkCore;

namespace EventCalendar.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<CalendarEvent> CalendarEvents => Set<CalendarEvent>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Casi todas las consultas filtran por fecha de inicio, así que la indexamos
            modelBuilder.Entity<CalendarEvent>()
                .HasIndex(e => e.StartTime);
        }
    }
}
