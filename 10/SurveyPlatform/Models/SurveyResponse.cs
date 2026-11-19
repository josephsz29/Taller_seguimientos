namespace SurveyPlatform.Models
{
    // Una persona que respondió la encuesta (un envío del formulario)
    public class SurveyResponse
    {
        public int Id { get; set; }

        public int SurveyId { get; set; }
        public Survey Survey { get; set; } = null!;

        public DateTime SubmittedAt { get; set; } = DateTime.Now;

        public List<Answer> Answers { get; set; } = new();
    }
}
