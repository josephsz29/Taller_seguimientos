using System.Security.Cryptography;
using BookingSystem.Data;
using BookingSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.Services
{
    public class BookingService : IBookingService
    {
        // Sin caracteres ambiguos (I, O, 0, 1) para que el código sea fácil de leer
        private const string CodeAlphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        private const int CodeLength = 8;

        private readonly AppDbContext _context;

        public BookingService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<BookableService>> GetServicesAsync()
        {
            return await _context.BookableServices
                .OrderBy(s => s.Id)
                .ToListAsync();
        }

        public async Task<BookableService?> GetServiceAsync(int id)
        {
            return await _context.BookableServices.FindAsync(id);
        }

        public async Task<List<TimeSlot>> GetAvailableSlotsAsync(int serviceId, DateTime date)
        {
            var slots = new List<TimeSlot>();

            var service = await GetServiceAsync(serviceId);
            if (service == null || !BookingRules.IsBusinessDay(date))
            {
                return slots;
            }

            var dayStart = date.Date + BookingRules.OpeningTime;
            var dayEnd = date.Date + BookingRules.ClosingTime;

            // Reservas que ocupan horario ese día
            var occupied = await ActiveBookings()
                .Where(b => b.StartTime < dayEnd && b.EndTime > dayStart)
                .Select(b => new { b.StartTime, b.EndTime })
                .ToListAsync();

            var now = DateTime.Now;
            var duration = TimeSpan.FromMinutes(service.DurationMinutes);

            // Generamos un horario cada "duración del servicio" dentro del horario de atención
            for (var start = dayStart; start + duration <= dayEnd; start += duration)
            {
                var end = start + duration;

                // Dos intervalos se cruzan si: inicioA < finB  y  finA > inicioB
                var overlaps = occupied.Any(o => o.StartTime < end && o.EndTime > start);

                slots.Add(new TimeSlot
                {
                    StartTime = start,
                    EndTime = end,
                    IsAvailable = start > now && !overlaps
                });
            }

            return slots;
        }

        public async Task<bool> IsSlotAvailableAsync(int serviceId, DateTime startTime)
        {
            var slots = await GetAvailableSlotsAsync(serviceId, startTime.Date);
            return slots.Any(s => s.IsAvailable && s.StartTime == startTime);
        }

        public async Task<Booking?> CreatePendingAsync(int serviceId, DateTime startTime, string customerName, string customerEmail, string? customerPhone)
        {
            var service = await GetServiceAsync(serviceId);

            // Verificamos de nuevo en el servidor: el horario pudo ocuparse mientras el usuario llenaba el formulario
            if (service == null || !await IsSlotAvailableAsync(serviceId, startTime))
            {
                return null;
            }

            var booking = new Booking
            {
                ServiceId = service.Id,
                CustomerName = customerName,
                CustomerEmail = customerEmail,
                CustomerPhone = string.IsNullOrWhiteSpace(customerPhone) ? null : customerPhone,
                StartTime = startTime,
                EndTime = startTime.AddMinutes(service.DurationMinutes),
                Status = BookingStatus.Pending,
                Code = await GenerateUniqueCodeAsync()
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return booking;
        }

        public async Task<Booking?> GetByCodeAsync(string code)
        {
            var normalizedCode = code.Trim().ToUpperInvariant();

            return await _context.Bookings
                .Include(b => b.Service)
                .FirstOrDefaultAsync(b => b.Code == normalizedCode);
        }

        public async Task<bool> ConfirmAsync(string code)
        {
            var booking = await GetByCodeAsync(code);

            if (booking == null || booking.Status != BookingStatus.Pending)
            {
                return false;
            }

            // No se puede confirmar un horario que ya pasó
            if (booking.StartTime <= DateTime.Now)
            {
                return false;
            }

            // Si el tiempo de espera venció, otra persona pudo tomar el horario
            if (await IsSlotTakenAsync(booking.StartTime, booking.EndTime, booking.Id))
            {
                return false;
            }

            booking.Status = BookingStatus.Confirmed;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> CancelAsync(string code)
        {
            var booking = await GetByCodeAsync(code);

            if (booking == null || booking.Status == BookingStatus.Cancelled)
            {
                return false;
            }

            booking.Status = BookingStatus.Cancelled;
            await _context.SaveChangesAsync();

            return true;
        }

        // Reservas que bloquean un horario: las confirmadas y las pendientes que aún no vencen
        private IQueryable<Booking> ActiveBookings()
        {
            var holdLimit = DateTime.Now.AddMinutes(-BookingRules.PendingHoldMinutes);

            return _context.Bookings.Where(b =>
                b.Status == BookingStatus.Confirmed ||
                (b.Status == BookingStatus.Pending && b.CreatedAt > holdLimit));
        }

        private async Task<bool> IsSlotTakenAsync(DateTime start, DateTime end, int excludedBookingId)
        {
            return await ActiveBookings()
                .AnyAsync(b => b.Id != excludedBookingId && b.StartTime < end && b.EndTime > start);
        }

        private async Task<string> GenerateUniqueCodeAsync()
        {
            while (true)
            {
                var chars = new char[CodeLength];
                for (int i = 0; i < CodeLength; i++)
                {
                    // RandomNumberGenerator es criptográficamente seguro (el código no debe ser adivinable)
                    chars[i] = CodeAlphabet[RandomNumberGenerator.GetInt32(CodeAlphabet.Length)];
                }

                var code = new string(chars);

                if (!await _context.Bookings.AnyAsync(b => b.Code == code))
                {
                    return code;
                }
            }
        }
    }
}
