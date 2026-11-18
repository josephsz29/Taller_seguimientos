using Microsoft.EntityFrameworkCore;
using RecipeHub.Models;

namespace RecipeHub.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<FoodType> FoodTypes => Set<FoodType>();
        public DbSet<Recipe> Recipes => Set<Recipe>();
        public DbSet<RecipeIngredient> RecipeIngredients => Set<RecipeIngredient>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // El código para compartir no se puede repetir
            modelBuilder.Entity<Recipe>()
                .HasIndex(r => r.ShareCode)
                .IsUnique();

            // No se puede borrar un tipo de comida que ya tiene recetas
            modelBuilder.Entity<Recipe>()
                .HasOne(r => r.FoodType)
                .WithMany(f => f.Recipes)
                .HasForeignKey(r => r.FoodTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Al borrar una receta se borran también sus ingredientes (cascada, es el comportamiento por defecto)
            modelBuilder.Entity<RecipeIngredient>()
                .HasOne(i => i.Recipe)
                .WithMany(r => r.Ingredients)
                .HasForeignKey(i => i.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);

            // Datos iniciales: se insertan al aplicar la migración (Update-Database)
            modelBuilder.Entity<FoodType>().HasData(
                new FoodType { Id = 1, Name = "Desayuno" },
                new FoodType { Id = 2, Name = "Almuerzo" },
                new FoodType { Id = 3, Name = "Cena" },
                new FoodType { Id = 4, Name = "Postre" },
                new FoodType { Id = 5, Name = "Snack" },
                new FoodType { Id = 6, Name = "Bebida" });
        }
    }
}
