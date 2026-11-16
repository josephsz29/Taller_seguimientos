using Microsoft.AspNetCore.Mvc;
using TodoList.Models;
using TodoList.Repositories;

namespace TodoList.Controllers
{
    public class TodoController : Controller
    {
        private readonly ITodoRepository _repository;

        public TodoController(ITodoRepository repository)
        {
            _repository = repository;
        }

        // GET: /Todo
        public async Task<IActionResult> Index()
        {
            var items = await _repository.GetAllAsync();
            return View(items);
        }

        // POST: /Todo/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                TempData["Error"] = "El título no puede estar vacío.";
                return RedirectToAction(nameof(Index));
            }

            await _repository.AddAsync(new TodoItem { Title = title.Trim() });
            return RedirectToAction(nameof(Index));
        }

        // POST: /Todo/Toggle/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Toggle(int id)
        {
            await _repository.ToggleCompletedAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // POST: /Todo/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _repository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Error()
        {
            return Content("Ocurrió un error inesperado.");
        }
    }
}
