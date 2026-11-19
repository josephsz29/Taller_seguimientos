using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace SurveyPlatform.Models
{
    // Lo que se muestra y lo que responde el usuario en UNA pregunta del formulario público
    public class QuestionAnswerInput
    {
        public int QuestionId { get; set; }

        // Estos datos vienen de la base de datos, NUNCA del formulario
        [BindNever]
        public string Text { get; set; } = string.Empty;

        [BindNever]
        public QuestionType Type { get; set; }

        [BindNever]
        public bool IsRequired { get; set; }

        [BindNever]
        public List<QuestionOption> Options { get; set; } = new();

        [BindNever]
        public string? Error { get; set; }

        // Lo que respondió el usuario (según el tipo de pregunta se usa uno u otro)
        public int? SelectedOptionId { get; set; }
        public List<int> SelectedOptionIds { get; set; } = new();
        public int? RatingValue { get; set; }
        public string? TextValue { get; set; }
    }
}
