using Microsoft.AspNetCore.Mvc;
using TipCalculator.Models;
using TipCalculator.Services;

namespace TipCalculator.Controllers
{
    public class TipController : Controller
    {
        private readonly ITipService _tipService;

        public TipController(ITipService tipService)
        {
            _tipService = tipService;
        }

        // GET: /Tip  -> muestra el formulario vacío y el historial
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = new TipViewModel
            {
                History = await _tipService.GetHistoryAsync()
            };
            return View(model);
        }

        // POST: /Tip  -> valida, calcula, guarda y muestra el resultado
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(TipViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.Result = await _tipService.CalculateAndSaveAsync(
                    model.BillAmount!.Value,
                    model.TipPercentage);
            }

            model.History = await _tipService.GetHistoryAsync();
            return View(model);
        }

        public IActionResult Error()
        {
            return Content("Ocurrió un error inesperado.");
        }
    }
}
