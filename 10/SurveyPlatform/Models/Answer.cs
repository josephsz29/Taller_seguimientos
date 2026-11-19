using System.ComponentModel.DataAnnotations;

namespace SurveyPlatform.Models
{
    // La respuesta a UNA pregunta. Según el tipo de pregunta se usa un campo distinto:
    //  - opción única / múltiple: OptionId (en opción múltiple hay una fila por cada opción marcada)
    //  - calificación: RatingValue
    //  - texto libre: TextValue
    public class Answer
    {
        public int Id { get; set; }

        public int SurveyResponseId { get; set; }
        public SurveyResponse SurveyResponse { get; set; } = null!;

        public int QuestionId { get; set; }
        public Question Question { get; set; } = null!;

        public int? OptionId { get; set; }
        public QuestionOption? Option { get; set; }

        public int? RatingValue { get; set; }

        [StringLength(1000)]
        public string? TextValue { get; set; }
    }
}
