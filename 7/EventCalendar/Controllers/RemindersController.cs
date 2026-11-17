using EventCalendar.Services;
using Microsoft.AspNetCore.Mvc;

namespace EventCalendar.Controllers
{
    // Lo usa el JavaScript de la página para consultar y cerrar recordatorios sin recargar
    public class RemindersController : Controller
    {
        private readonly IEventService _eventService;

        public RemindersController(IEventService eventService)
        {
            _eventService = eventService;
        }

        // GET: /Reminders/Due  -> recordatorios que ya deben mostrarse (JSON)
        [HttpGet]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> Due()
        {
            var now = DateTime.Now;
            var reminders = await _eventService.GetDueRemindersAsync(now);

            var result = reminders.Select(e => new
            {
                id = e.Id,
                title = e.Title,
                startTime = e.StartTime.ToString("dd/MM/yyyy HH:mm"),
                minutesLeft = (int)Math.Ceiling((e.StartTime - now).TotalMinutes)
            });

            return Json(result);
        }

        // POST: /Reminders/Dismiss  -> el usuario cerró el aviso
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Dismiss(int id)
        {
            var dismissed = await _eventService.DismissReminderAsync(id);
            return dismissed ? Ok() : NotFound();
        }
    }
}
