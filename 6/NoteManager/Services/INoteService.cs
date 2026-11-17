using NoteManager.Models;

namespace NoteManager.Services
{
    public interface INoteService
    {
        Task<List<Note>> SearchAsync(string? search, int? categoryId);
        Task<Note?> GetByIdAsync(int id);
        Task AddAsync(Note note);
        Task<bool> UpdateAsync(int id, string title, string content, int categoryId);
        Task DeleteAsync(int id);
    }
}
