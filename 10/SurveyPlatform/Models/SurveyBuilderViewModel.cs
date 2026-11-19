namespace SurveyPlatform.Models
{
    // Datos de la página donde se arman las preguntas
    public class SurveyBuilderViewModel
    {
        public Survey Survey { get; set; } = null!;
        public int ResponseCount { get; set; }
        public QuestionFormViewModel NewQuestion { get; set; } = new();

        // Cuando ya hay respuestas, las preguntas no se pueden cambiar (los resultados quedarían inconsistentes)
        public bool CanEditQuestions => ResponseCount == 0;
    }
}
