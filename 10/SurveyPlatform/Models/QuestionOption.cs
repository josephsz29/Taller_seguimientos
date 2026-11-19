using System.ComponentModel.DataAnnotations;

namespace SurveyPlatform.Models
{
    public class QuestionOption
    {
        public int Id { get; set; }

        public int QuestionId { get; set; }
        public Question Question { get; set; } = null!;

        [Required]
        [StringLength(150)]
        public string Text { get; set; } = string.Empty;

        public int Position { get; set; }
    }
}
