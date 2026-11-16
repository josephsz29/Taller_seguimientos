using Microsoft.EntityFrameworkCore;
using TodoList.Models;

namespace TodoList.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Representa la tabla TodoItems en la base de datos
        public DbSet<TodoItem> TodoItems => Set<TodoItem>();
    }
}
