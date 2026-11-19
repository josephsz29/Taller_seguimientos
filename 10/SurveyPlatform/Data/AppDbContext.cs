using Microsoft.EntityFrameworkCore;
using SurveyPlatform.Models;

namespace SurveyPlatform.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Survey> Surveys => Set<Survey>();
        public DbSet<Question> Questions => Set<Question>();
        public DbSet<QuestionOption> QuestionOptions => Set<QuestionOption>();
        public DbSet<SurveyResponse> SurveyResponses => Set<SurveyResponse>();
        public DbSet<Answer> Answers => Set<Answer>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // El código del enlace público no se puede repetir
            modelBuilder.Entity<Survey>()
                .HasIndex(s => s.ShareCode)
                .IsUnique();

            // Una respuesta apunta a su pregunta y a la opción elegida. Estas dos relaciones NO borran en cascada:
            // SQL Server no permite varios caminos de borrado en cascada hacia la misma tabla
            // (las respuestas ya se borran en cascada desde SurveyResponse).
            modelBuilder.Entity<Answer>()
                .HasOne(a => a.Question)
                .WithMany()
                .HasForeignKey(a => a.QuestionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Answer>()
                .HasOne(a => a.Option)
                .WithMany()
                .HasForeignKey(a => a.OptionId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
