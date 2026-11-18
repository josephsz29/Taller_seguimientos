using MemoryGame.Models;
using Microsoft.EntityFrameworkCore;

namespace MemoryGame.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Game> Games => Set<Game>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // El ranking consulta por nivel y ordena por puntaje: un índice compuesto lo acelera
            modelBuilder.Entity<Game>()
                .HasIndex(g => new { g.Difficulty, g.Score });
        }
    }
}
