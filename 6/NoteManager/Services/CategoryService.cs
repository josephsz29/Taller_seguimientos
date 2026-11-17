using Microsoft.EntityFrameworkCore;
using NoteManager.Data;
using NoteManager.Models;

namespace NoteManager.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly AppDbContext _context;

        public CategoryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Category>> GetAllAsync()
        {
            return await _context.Categories
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<List<CategorySummary>> GetSummariesAsync()
        {
            // EF convierte c.Notes.Count en un COUNT dentro de la misma consulta SQL
            return await _context.Categories
                .OrderBy(c => c.Name)
                .Select(c => new CategorySummary
                {
                    Id = c.Id,
                    Name = c.Name,
                    NoteCount = c.Notes.Count
                })
                .ToListAsync();
        }

        public async Task<bool> AddAsync(string name)
        {
            var trimmedName = name.Trim();

            // SQL Server compara sin distinguir mayúsculas, así que "ideas" e "Ideas" cuentan como iguales
            if (await _context.Categories.AnyAsync(c => c.Name == trimmedName))
            {
                return false;
            }

            _context.Categories.Add(new Category { Name = trimmedName });
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            // No se puede borrar una categoría que tiene notas
            if (await _context.Notes.AnyAsync(n => n.CategoryId == id))
            {
                return false;
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return false;
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
