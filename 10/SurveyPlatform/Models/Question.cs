using System.ComponentModel.DataAnnotations;

namespace SurveyPlatform.Models
{
    public class Question
    {
        public int Id { get; set; }

        public int SurveyId { get; set; }
        public Survey Survey { get; set; } = null!;

        [Required]
        [StringLength(300)]
        public string Text { get; set; } = string.Empty;

        public QuestionType Type { get; set; }

        public bool IsRequired { get; set; }

        // Orden en que aparece la pregunta dentro de la encuesta
        public int Position { get; set; }

        // Opciones (solo para preguntas de opción única o múltiple)
        public List<QuestionOption> Options { get; set; } = new();
    }
}
