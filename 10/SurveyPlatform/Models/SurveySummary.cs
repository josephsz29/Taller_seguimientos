namespace SurveyPlatform.Models
{
    // Una encuesta con sus totales, para la lista principal (no se guarda en la base de datos)
    public class SurveySummary
    {
        public Survey Survey { get; set; } = null!;
        public int QuestionCount { get; set; }
        public int ResponseCount { get; set; }
    }
}
