namespace SurveyPlatform.Models
{
    // Cuántas personas eligieron una opción (o una calificación)
    public class OptionResult
    {
        public string Label { get; set; } = string.Empty;
        public int Count { get; set; }

        // Porcentaje sobre las personas que respondieron esa pregunta
        public decimal Percentage { get; set; }
    }
}
