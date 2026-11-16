using ExpenseTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Expense> Expenses => Set<Expense>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Datos iniciales: se insertan al aplicar la migración (Update-Database)
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Alimentación" },
                new Category { Id = 2, Name = "Transporte" },
                new Category { Id = 3, Name = "Vivienda" },
                new Category { Id = 4, Name = "Salud" },
                new Category { Id = 5, Name = "Entretenimiento" },
                new Category { Id = 6, Name = "Otros" });
        }
    }
}
