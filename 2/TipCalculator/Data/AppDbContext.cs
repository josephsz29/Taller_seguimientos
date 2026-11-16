using Microsoft.EntityFrameworkCore;
using TipCalculator.Models;

namespace TipCalculator.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Representa la tabla TipRecords en la base de datos
        public DbSet<TipRecord> TipRecords => Set<TipRecord>();
    }
}
