using System.Globalization;
using EventCalendar.Models;
using EventCalendar.Services;
using Microsoft.AspNetCore.Mvc;

namespace EventCalendar.Controllers
{
    public class EventsController : Controller
    {
        private readonly IEventService _eventService;

        public EventsController(IEventService eventService)
        {
            _eventService = eventService;
        }

        // GET: /Events?month=2026-11  -> calendario del mes + próximos eventos
        [HttpGet]
        public async Task<IActionResult> Index(string? month)
        {
            var selectedMonth = ParseMonth(month);

            var model = new CalendarIndexViewModel
            {
                Month = await _eventService.GetMonthAsync(selectedMonth.Year, selectedMonth.Month),
                Upcoming = await _eventService.GetUpcomingAsync(5)
            };

            return View(model);
        }

        // GET: /Events/Create?date=2026-11-20  -> formulario (el día se puede preseleccionar)
        [HttpGet]
        public IActionResult Create(string? date)
        {
            var start = GetDefaultStart(date);

            var model = new EventFormViewModel
            {
                StartTime = start,
                EndTime = start.AddHours(1),
                ReminderMinutes = 15
            };

            return View("Form", model);
        }

        // POST: /Events/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EventFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Form", model);
            }

            await _eventService.AddAsync(new CalendarEvent
            {
                Title = model.Title.Trim(),
                Description = string.IsNullOrWhiteSpace(model.Description) ? null : model.Description.Trim(),
                StartTime = model.StartTime,
                EndTime = model.EndTime,
                ReminderMinutes = model.ReminderMinutes
            });

            TempData["Success"] = "Evento guardado.";
            return RedirectToAction(nameof(Index), new { month = model.MonthValue });
        }

        // GET: /Events/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var calendarEvent = await _eventService.GetByIdAsync(id);
            if (calendarEvent == null)
            {
                return NotFound();
            }

            var model = new EventFormViewModel
            {
                Id = calendarEvent.Id,
                Title = calendarEvent.Title,
                Description = calendarEvent.Description,
                StartTime = calendarEvent.StartTime,
                EndTime = calendarEvent.EndTime,
                ReminderMinutes = calendarEvent.ReminderMinutes
            };

            return View("Form", model);
        }

        // POST: /Events/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EventFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Form", model);
            }

            var updated = await _eventService.UpdateAsync(
                model.Id,
                model.Title.Trim(),
                string.IsNullOrWhiteSpace(model.Description) ? null : model.Description.Trim(),
                model.StartTime,
                model.EndTime,
                model.ReminderMinutes);

            if (!updated)
            {
                return NotFound();
            }

            TempData["Success"] = "Evento actualizado.";
            return RedirectToAction(nameof(Index), new { month = model.MonthValue });
        }

        // POST: /Events/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, string? month)
        {
            await _eventService.DeleteAsync(id);

            TempData["Success"] = "Evento eliminado.";
            return RedirectToAction(nameof(Index), new { month });
        }

        public IActionResult Error()
        {
            return Content("Ocurrió un error inesperado.");
        }

        // Convierte "2026-11" en el primer día de ese mes; si no es válido, usa el mes actual
        private static DateTime ParseMonth(string? month)
        {
            if (!string.IsNullOrEmpty(month) &&
                DateTime.TryParseExact(month + "-01", "yyyy-MM-dd", CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out var parsed))
            {
                return parsed;
            }

            var today = DateTime.Today;
            return new DateTime(today.Year, today.Month, 1);
        }

        // Si viene un día, empieza a las 9:00; si no, en la próxima hora en punto
        private static DateTime GetDefaultStart(string? date)
        {
            if (!string.IsNullOrEmpty(date) &&
                DateTime.TryParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out var parsed))
            {
                return parsed.AddHours(9);
            }

            var now = DateTime.Now;
            return new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0).AddHours(1);
        }
    }
}
