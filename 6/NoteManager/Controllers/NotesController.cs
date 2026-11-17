using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NoteManager.Models;
using NoteManager.Services;

namespace NoteManager.Controllers
{
    public class NotesController : Controller
    {
        private readonly INoteService _noteService;
        private readonly ICategoryService _categoryService;

        public NotesController(INoteService noteService, ICategoryService categoryService)
        {
            _noteService = noteService;
            _categoryService = categoryService;
        }

        // GET: /Notes?search=examen&categoryId=3  -> lista de notas con búsqueda y filtro
        [HttpGet]
        public async Task<IActionResult> Index(string? search, int? categoryId)
        {
            var model = new NoteIndexViewModel
            {
                Search = search,
                CategoryId = categoryId,
                Notes = await _noteService.SearchAsync(search, categoryId),
                Categories = await GetCategoryItemsAsync()
            };

            return View(model);
        }

        // GET: /Notes/Details/5  -> nota completa
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var note = await _noteService.GetByIdAsync(id);
            if (note == null)
            {
                return NotFound();
            }

            return View(note);
        }

        // GET: /Notes/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new NoteFormViewModel
            {
                Categories = await GetCategoryItemsAsync()
            };

            return View("Form", model);
        }

        // POST: /Notes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NoteFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = await GetCategoryItemsAsync();
                return View("Form", model);
            }

            await _noteService.AddAsync(new Note
            {
                Title = model.Title.Trim(),
                Content = model.Content.Trim(),
                CategoryId = model.CategoryId
            });

            TempData["Success"] = "Nota guardada.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Notes/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var note = await _noteService.GetByIdAsync(id);
            if (note == null)
            {
                return NotFound();
            }

            var model = new NoteFormViewModel
            {
                Id = note.Id,
                Title = note.Title,
                Content = note.Content,
                CategoryId = note.CategoryId,
                Categories = await GetCategoryItemsAsync()
            };

            return View("Form", model);
        }

        // POST: /Notes/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(NoteFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = await GetCategoryItemsAsync();
                return View("Form", model);
            }

            var updated = await _noteService.UpdateAsync(
                model.Id,
                model.Title.Trim(),
                model.Content.Trim(),
                model.CategoryId);

            if (!updated)
            {
                return NotFound();
            }

            TempData["Success"] = "Nota actualizada.";
            return RedirectToAction(nameof(Details), new { id = model.Id });
        }

        // POST: /Notes/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _noteService.DeleteAsync(id);

            TempData["Success"] = "Nota eliminada.";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Error()
        {
            return Content("Ocurrió un error inesperado.");
        }

        private async Task<List<SelectListItem>> GetCategoryItemsAsync()
        {
            var categories = await _categoryService.GetAllAsync();

            return categories
                .Select(c => new SelectListItem(c.Name, c.Id.ToString()))
                .ToList();
        }
    }
}
