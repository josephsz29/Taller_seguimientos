using EventCalendar.Data;
using EventCalendar.Models;
using Microsoft.EntityFrameworkCore;

namespace EventCalendar.Services
{
    public class EventService : IEventService
    {
        private readonly AppDbContext _context;

        public EventService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<CalendarMonth> GetMonthAsync(int year, int month)
        {
            var firstOfMonth = new DateTime(year, month, 1);

            // La semana empieza en lunes: lunes = 0 ... domingo = 6
            var offset = ((int)firstOfMonth.DayOfWeek + 6) % 7;
            var gridStart = firstOfMonth.AddDays(-offset);
            var gridEnd = gridStart.AddDays(42);

            // Una sola consulta trae los eventos de toda la cuadrícula
            var events = await _context.CalendarEvents
                .Where(e => e.StartTime >= gridStart && e.StartTime < gridEnd)
                .OrderBy(e => e.StartTime)
                .ToListAsync();

            var today = DateTime.Today;
            var days = new List<CalendarDay>();

            for (int i = 0; i < 42; i++)
            {
                var date = gridStart.AddDays(i);

                days.Add(new CalendarDay
                {
                    Date = date,
                    IsCurrentMonth = date.Month == month,
                    IsToday = date == today,
                    Events = events.Where(e => e.StartTime.Date == date).ToList()
                });
            }

            return new CalendarMonth { Year = year, Month = month, Days = days };
        }

        public async Task<List<CalendarEvent>> GetUpcomingAsync(int count)
        {
            var now = DateTime.Now;

            return await _context.CalendarEvents
                .Where(e => e.EndTime >= now)
                .OrderBy(e => e.StartTime)
                .Take(count)
                .ToListAsync();
        }

        public async Task<CalendarEvent?> GetByIdAsync(int id)
        {
            return await _context.CalendarEvents.FindAsync(id);
        }

        public async Task AddAsync(CalendarEvent calendarEvent)
        {
            _context.CalendarEvents.Add(calendarEvent);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(int id, string title, string? description, DateTime startTime, DateTime endTime, int? reminderMinutes)
        {
            var calendarEvent = await _context.CalendarEvents.FindAsync(id);
            if (calendarEvent == null)
            {
                return false;
            }

            // Si cambió la hora de inicio o el recordatorio, el aviso vuelve a activarse
            if (calendarEvent.StartTime != startTime || calendarEvent.ReminderMinutes != reminderMinutes)
            {
                calendarEvent.ReminderDismissed = false;
            }

            calendarEvent.Title = title;
            calendarEvent.Description = description;
            calendarEvent.StartTime = startTime;
            calendarEvent.EndTime = endTime;
            calendarEvent.ReminderMinutes = reminderMinutes;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task DeleteAsync(int id)
        {
            var calendarEvent = await _context.CalendarEvents.FindAsync(id);
            if (calendarEvent != null)
            {
                _context.CalendarEvents.Remove(calendarEvent);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<CalendarEvent>> GetDueRemindersAsync(DateTime now)
        {
            // La base de datos descarta lo que seguro no aplica: sin recordatorio, ya cerrado o ya iniciado
            var candidates = await _context.CalendarEvents
                .Where(e => e.ReminderMinutes != null && !e.ReminderDismissed && e.StartTime > now)
                .ToListAsync();

            // Luego, en memoria, dejamos los que ya llegaron a su hora de aviso
            return candidates
                .Where(e => e.ReminderTime <= now)
                .OrderBy(e => e.StartTime)
                .ToList();
        }

        public async Task<bool> DismissReminderAsync(int id)
        {
            var calendarEvent = await _context.CalendarEvents.FindAsync(id);
            if (calendarEvent == null)
            {
                return false;
            }

            calendarEvent.ReminderDismissed = true;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
