using System.Text.Json;

namespace SurveyPlatform.Models
{
    // Resultados de una pregunta
    public class QuestionResult
    {
        public int QuestionId { get; set; }
        public string Text { get; set; } = string.Empty;
        public QuestionType Type { get; set; }

        // Personas que respondieron esta pregunta
        public int AnsweredCount { get; set; }

        // Opciones o calificaciones con su conteo (preguntas de opción y de calificación)
        public List<OptionResult> Options { get; set; } = new();

        // Solo para calificaciones
        public decimal? RatingAverage { get; set; }

        // Solo para texto libre
        public List<string> TextAnswers { get; set; } = new();

        // Datos para el gráfico, como texto JSON (se leen desde JavaScript)
        public string LabelsJson => JsonSerializer.Serialize(Options.Select(o => o.Label));
        public string ValuesJson => JsonSerializer.Serialize(Options.Select(o => o.Count));

        // Tipo de gráfico según el tipo de pregunta
        public string ChartType => Type switch
        {
            QuestionType.SingleChoice => "doughnut",
            QuestionType.MultipleChoice => "bar-horizontal",
            _ => "bar"
        };
    }
}
