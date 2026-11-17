using Microsoft.EntityFrameworkCore;
using NoteManager.Models;

namespace NoteManager.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Note> Notes => Set<Note>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // No pueden existir dos categorías con el mismo nombre
            modelBuilder.Entity<Category>()
                .HasIndex(c => c.Name)
                .IsUnique();

            // No se puede borrar una categoría que todavía tiene notas
            modelBuilder.Entity<Note>()
                .HasOne(n => n.Category)
                .WithMany(c => c.Notes)
                .HasForeignKey(n => n.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Datos iniciales: se insertan al aplicar la migración (Update-Database)
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Personal" },
                new Category { Id = 2, Name = "Trabajo" },
                new Category { Id = 3, Name = "Estudio" },
                new Category { Id = 4, Name = "Ideas" },
                new Category { Id = 5, Name = "Otros" });
        }
    }
}
