using NoteManager.Models;

namespace NoteManager.Services
{
    public interface ICategoryService
    {
        Task<List<Category>> GetAllAsync();
        Task<List<CategorySummary>> GetSummariesAsync();
        Task<bool> AddAsync(string name);
        Task<bool> DeleteAsync(int id);
    }
}
