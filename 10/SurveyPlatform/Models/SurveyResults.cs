namespace SurveyPlatform.Models
{
    public class SurveyResults
    {
        public Survey Survey { get; set; } = null!;
        public int TotalResponses { get; set; }
        public List<QuestionResult> Questions { get; set; } = new();
    }
}
