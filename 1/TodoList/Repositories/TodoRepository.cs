using Microsoft.EntityFrameworkCore;
using TodoList.Data;
using TodoList.Models;

namespace TodoList.Repositories
{
    public class TodoRepository : ITodoRepository
    {
        private readonly AppDbContext _context;

        public TodoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<TodoItem>> GetAllAsync()
        {
            // Primero las pendientes y, dentro de cada grupo, las más recientes
            return await _context.TodoItems
                .OrderBy(t => t.IsCompleted)
                .ThenByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task AddAsync(TodoItem item)
        {
            _context.TodoItems.Add(item);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var item = await _context.TodoItems.FindAsync(id);
            if (item != null)
            {
                _context.TodoItems.Remove(item);
                await _context.SaveChangesAsync();
            }
        }

        public async Task ToggleCompletedAsync(int id)
        {
            var item = await _context.TodoItems.FindAsync(id);
            if (item != null)
            {
                item.IsCompleted = !item.IsCompleted;
                await _context.SaveChangesAsync();
            }
        }
    }
}
