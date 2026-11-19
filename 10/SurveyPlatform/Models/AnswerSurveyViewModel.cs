using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace SurveyPlatform.Models
{
    // Datos de la página pública donde se responde la encuesta
    public class AnswerSurveyViewModel
    {
        public string Code { get; set; } = string.Empty;

        [BindNever]
        public string Title { get; set; } = string.Empty;

        [BindNever]
        public string? Description { get; set; }

        [BindNever]
        public string? GeneralError { get; set; }

        public List<QuestionAnswerInput> Questions { get; set; } = new();
    }
}
