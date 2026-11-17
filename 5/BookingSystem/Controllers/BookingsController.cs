using System.Globalization;
using BookingSystem.Models;
using BookingSystem.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookingSystem.Controllers
{
    public class BookingsController : Controller
    {
        private readonly IBookingService _bookingService;

        public BookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        // GET: /Bookings  -> lista de servicios y búsqueda por código
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var services = await _bookingService.GetServicesAsync();
            return View(services);
        }

        // GET: /Bookings/Slots?serviceId=1&date=2026-11-17  -> horarios con su disponibilidad
        [HttpGet]
        public async Task<IActionResult> Slots(int serviceId, string? date)
        {
            var service = await _bookingService.GetServiceAsync(serviceId);
            if (service == null)
            {
                return NotFound();
            }

            var selectedDate = ParseDate(date);

            var model = new SlotsViewModel
            {
                Service = service,
                Date = selectedDate,
                Slots = await _bookingService.GetAvailableSlotsAsync(serviceId, selectedDate)
            };

            return View(model);
        }

        // GET: /Bookings/Create?serviceId=1&start=2026-11-17T09:00  -> formulario de datos
        [HttpGet]
        public async Task<IActionResult> Create(int serviceId, DateTime start)
        {
            var service = await _bookingService.GetServiceAsync(serviceId);
            if (service == null)
            {
                return NotFound();
            }

            if (!await _bookingService.IsSlotAvailableAsync(serviceId, start))
            {
                TempData["Error"] = "Ese horario ya no está disponible. Elige otro.";
                return RedirectToAction(nameof(Slots), new { serviceId, date = start.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) });
            }

            return View(new BookingFormViewModel
            {
                ServiceId = serviceId,
                StartTime = start,
                Service = service
            });
        }

        // POST: /Bookings/Create  -> crea la reserva en estado pendiente
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookingFormViewModel form)
        {
            var service = await _bookingService.GetServiceAsync(form.ServiceId);
            if (service == null)
            {
                return NotFound();
            }

            form.Service = service;

            if (!ModelState.IsValid)
            {
                return View(form);
            }

            var booking = await _bookingService.CreatePendingAsync(
                form.ServiceId,
                form.StartTime,
                form.CustomerName.Trim(),
                form.CustomerEmail.Trim(),
                form.CustomerPhone?.Trim());

            if (booking == null)
            {
                TempData["Error"] = "Ese horario ya no está disponible. Elige otro.";
                return RedirectToAction(nameof(Slots), new { serviceId = form.ServiceId, date = form.DateValue });
            }

            return RedirectToAction(nameof(Details), new { code = booking.Code });
        }

        // GET: /Bookings/Details?code=AB12CD34  -> resumen y estado de la reserva
        [HttpGet]
        public async Task<IActionResult> Details(string? code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return RedirectToAction(nameof(Index));
            }

            var booking = await _bookingService.GetByCodeAsync(code);
            if (booking == null)
            {
                TempData["Error"] = "No encontramos una reserva con ese código.";
                return RedirectToAction(nameof(Index));
            }

            return View(booking);
        }

        // POST: /Bookings/Confirm  -> pasa la reserva de pendiente a confirmada
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Confirm(string code)
        {
            var confirmed = await _bookingService.ConfirmAsync(code);

            if (confirmed)
            {
                TempData["Success"] = "¡Tu reserva quedó confirmada!";
            }
            else
            {
                TempData["Error"] = "No se pudo confirmar la reserva: el horario ya fue tomado o la reserva ya no está pendiente.";
            }

            return RedirectToAction(nameof(Details), new { code });
        }

        // POST: /Bookings/Cancel
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(string code)
        {
            var cancelled = await _bookingService.CancelAsync(code);

            if (cancelled)
            {
                TempData["Success"] = "La reserva fue cancelada.";
            }
            else
            {
                TempData["Error"] = "No se pudo cancelar la reserva.";
            }

            return RedirectToAction(nameof(Details), new { code });
        }

        public IActionResult Error()
        {
            return Content("Ocurrió un error inesperado.");
        }

        // Convierte "2026-11-17" en fecha. Nunca devuelve una fecha pasada.
        // Si no hay fecha, usa hoy o el siguiente día hábil.
        private static DateTime ParseDate(string? date)
        {
            var today = DateTime.Today;

            if (!string.IsNullOrEmpty(date) &&
                DateTime.TryParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out var parsed))
            {
                return parsed < today ? today : parsed;
            }

            var next = today;
            while (!BookingRules.IsBusinessDay(next))
            {
                next = next.AddDays(1);
            }

            return next;
        }
    }
}
