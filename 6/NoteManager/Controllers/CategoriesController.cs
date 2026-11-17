using Microsoft.AspNetCore.Mvc;
using NoteManager.Models;
using NoteManager.Services;

namespace NoteManager.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // GET: /Categories  -> lista de categorías y formulario para agregar
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = new CategoryIndexViewModel
            {
                Categories = await _categoryService.GetSummariesAsync()
            };

            return View(model);
        }

        // POST: /Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryIndexViewModel model)
        {
            if (ModelState.IsValid)
            {
                var created = await _categoryService.AddAsync(model.Name);

                if (created)
                {
                    TempData["Success"] = "Categoría agregada.";
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError(nameof(model.Name), "Ya existe una categoría con ese nombre.");
            }

            // Si hubo errores, volvemos a mostrar la página con la lista actualizada
            model.Categories = await _categoryService.GetSummariesAsync();
            return View(nameof(Index), model);
        }

        // POST: /Categories/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _categoryService.DeleteAsync(id);

            if (deleted)
            {
                TempData["Success"] = "Categoría eliminada.";
            }
            else
            {
                TempData["Error"] = "No se puede eliminar: la categoría todavía tiene notas.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
