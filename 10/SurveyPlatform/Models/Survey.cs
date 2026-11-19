using System.ComponentModel.DataAnnotations;

namespace SurveyPlatform.Models
{
    public class Survey
    {
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Title { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        // Código aleatorio para el enlace público (no se exponen los Id consecutivos)
        [Required]
        [StringLength(10)]
        public string ShareCode { get; set; } = string.Empty;

        // Solo se puede responder una encuesta abierta
        public bool IsOpen { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Una encuesta tiene muchas preguntas y muchas respuestas
        public List<Question> Questions { get; set; } = new();
        public List<SurveyResponse> Responses { get; set; } = new();
    }
}
