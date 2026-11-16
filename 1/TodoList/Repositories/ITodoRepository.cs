using TodoList.Models;

namespace TodoList.Repositories
{
    public interface ITodoRepository
    {
        Task<List<TodoItem>> GetAllAsync();
        Task AddAsync(TodoItem item);
        Task DeleteAsync(int id);
        Task ToggleCompletedAsync(int id);
    }
}
