using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using OnlineChronometer.Models;
using OnlineChronometer.Services;

namespace OnlineChronometer.Controllers
{
    public class ChronometersController : Controller
    {
        private const int MaxNameLength = 60;

        private readonly IChronometerService _chronometerService;

        public ChronometersController(IChronometerService chronometerService)
        {
            _chronometerService = chronometerService;
        }

        // GET: /Chronometers  -> lista de cronómetros y formulario para crear uno
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var summaries = await _chronometerService.GetSummariesAsync();
            return View(summaries);
        }

        // POST: /Chronometers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string? name)
        {
            var trimmedName = name?.Trim();

            if (trimmedName != null && trimmedName.Length > MaxNameLength)
            {
                TempData["Error"] = "El nombre no puede superar " + MaxNameLength + " caracteres.";
                return RedirectToAction(nameof(Index));
            }

            var chronometer = await _chronometerService.CreateAsync(trimmedName);
            return RedirectToAction(nameof(Details), new { id = chronometer.Id });
        }

        // GET: /Chronometers/Details/{id}  -> la página del cronómetro
        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var state = await _chronometerService.GetStateAsync(id);
            if (state == null)
            {
                return NotFound();
            }

            var model = new ChronometerDetailsViewModel
            {
                Id = state.Id,
                Name = state.Name,

                // JsonSerializerDefaults.Web escribe los nombres en minúscula inicial (elapsedMilliseconds...)
                StateJson = JsonSerializer.Serialize(state, new JsonSerializerOptions(JsonSerializerDefaults.Web))
            };

            return View(model);
        }

        // POST: /Chronometers/Delete/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _chronometerService.DeleteAsync(id);

            TempData["Success"] = "Cronómetro eliminado.";
            return RedirectToAction(nameof(Index));
        }

        // ================= Acciones que usa el JavaScript (responden JSON) =================

        // GET: /Chronometers/State?id=...  -> estado actual (para sincronizar)
        [HttpGet]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> State(Guid id)
        {
            var state = await _chronometerService.GetStateAsync(id);
            return state == null ? NotFound() : Json(state);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Start(Guid id)
        {
            var state = await _chronometerService.StartAsync(id);
            return state == null ? NotFound() : Json(state);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Pause(Guid id)
        {
            var state = await _chronometerService.PauseAsync(id);
            return state == null ? NotFound() : Json(state);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reset(Guid id)
        {
            var state = await _chronometerService.ResetAsync(id);
            return state == null ? NotFound() : Json(state);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Lap(Guid id)
        {
            var state = await _chronometerService.LapAsync(id);
            return state == null ? NotFound() : Json(state);
        }

        public IActionResult Error()
        {
            return Content("Ocurrió un error inesperado.");
        }
    }
}
