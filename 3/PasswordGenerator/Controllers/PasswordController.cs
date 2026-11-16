using Microsoft.AspNetCore.Mvc;
using PasswordGenerator.Models;
using PasswordGenerator.Services;

namespace PasswordGenerator.Controllers
{
    public class PasswordController : Controller
    {
        private readonly IPasswordService _passwordService;

        public PasswordController(IPasswordService passwordService)
        {
            _passwordService = passwordService;
        }

        // GET: /Password  -> muestra el formulario con valores por defecto y el historial
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = new PasswordViewModel
            {
                History = await _passwordService.GetHistoryAsync()
            };
            return View(model);
        }

        // POST: /Password  -> valida, genera, guarda las opciones y muestra la contraseña
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(PasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.Result = await _passwordService.GenerateAndSaveAsync(model.ToOptions());
            }

            model.History = await _passwordService.GetHistoryAsync();
            return View(model);
        }

        public IActionResult Error()
        {
            return Content("Ocurrió un error inesperado.");
        }
    }
}
