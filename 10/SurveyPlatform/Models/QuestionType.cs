namespace SurveyPlatform.Models
{
    public enum QuestionType
    {
        SingleChoice = 0,
        MultipleChoice = 1,
        Rating = 2,
        Text = 3
    }

    public static class QuestionTypeExtensions
    {
        // Nombre del tipo de pregunta en español
        public static string ToLabel(this QuestionType type)
        {
            return type switch
            {
                QuestionType.SingleChoice => "Opción única",
                QuestionType.MultipleChoice => "Opción múltiple",
                QuestionType.Rating => "Calificación (1 a 5)",
                _ => "Texto libre"
            };
        }

        // Solo las preguntas de opción única y múltiple tienen lista de opciones
        public static bool HasOptions(this QuestionType type)
        {
            return type == QuestionType.SingleChoice || type == QuestionType.MultipleChoice;
        }
    }
}
