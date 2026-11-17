using Microsoft.EntityFrameworkCore;
using NoteManager.Data;
using NoteManager.Models;

namespace NoteManager.Services
{
    public class NoteService : INoteService
    {
        private readonly AppDbContext _context;

        public NoteService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Note>> SearchAsync(string? search, int? categoryId)
        {
            IQueryable<Note> query = _context.Notes.Include(n => n.Category);

            // Filtro por categoría (opcional)
            if (categoryId.HasValue)
            {
                query = query.Where(n => n.CategoryId == categoryId.Value);
            }

            // Búsqueda (opcional): cada palabra debe aparecer en el título o en el contenido
            if (!string.IsNullOrWhiteSpace(search))
            {
                var terms = search.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                foreach (var term in terms)
                {
                    query = query.Where(n => n.Title.Contains(term) || n.Content.Contains(term));
                }
            }

            // Las notas modificadas más recientemente primero
            return await query
                .OrderByDescending(n => n.UpdatedAt)
                .ToListAsync();
        }

        public async Task<Note?> GetByIdAsync(int id)
        {
            return await _context.Notes
                .Include(n => n.Category)
                .FirstOrDefaultAsync(n => n.Id == id);
        }

        public async Task AddAsync(Note note)
        {
            _context.Notes.Add(note);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(int id, string title, string content, int categoryId)
        {
            var note = await _context.Notes.FindAsync(id);
            if (note == null)
            {
                return false;
            }

            note.Title = title;
            note.Content = content;
            note.CategoryId = categoryId;
            note.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task DeleteAsync(int id)
        {
            var note = await _context.Notes.FindAsync(id);
            if (note != null)
            {
                _context.Notes.Remove(note);
                await _context.SaveChangesAsync();
            }
        }
    }
}
