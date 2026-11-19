using SurveyPlatform.Models;

namespace SurveyPlatform.Services
{
    public interface ISurveyService
    {
        Task<List<SurveySummary>> GetSummariesAsync();
        Task<Survey?> GetByIdAsync(int id);
        Task<Survey?> GetByCodeAsync(string code);
        Task<int> GetResponseCountAsync(int surveyId);

        Task<Survey> CreateAsync(string title, string? description);
        Task<bool> UpdateAsync(int id, string title, string? description);
        Task<bool> ToggleOpenAsync(int id);
        Task DeleteAsync(int id);

        Task<bool> AddQuestionAsync(int surveyId, string text, QuestionType type, bool isRequired, List<string> options);
        Task<bool> DeleteQuestionAsync(int questionId);

        Task<Dictionary<int, string>> SubmitResponseAsync(int surveyId, IEnumerable<QuestionAnswerInput> posted);

        Task<SurveyResults?> GetResultsAsync(int surveyId);
        Task<string?> BuildCsvAsync(int surveyId);
    }
}
