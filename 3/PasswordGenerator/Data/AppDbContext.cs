using Microsoft.EntityFrameworkCore;
using PasswordGenerator.Models;

namespace PasswordGenerator.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Representa la tabla PasswordRecords en la base de datos
        public DbSet<PasswordRecord> PasswordRecords => Set<PasswordRecord>();
    }
}
