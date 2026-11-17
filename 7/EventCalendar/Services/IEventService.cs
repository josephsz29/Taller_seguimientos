using EventCalendar.Models;

namespace EventCalendar.Services
{
    public interface IEventService
    {
        Task<CalendarMonth> GetMonthAsync(int year, int month);
        Task<List<CalendarEvent>> GetUpcomingAsync(int count);
        Task<CalendarEvent?> GetByIdAsync(int id);
        Task AddAsync(CalendarEvent calendarEvent);
        Task<bool> UpdateAsync(int id, string title, string? description, DateTime startTime, DateTime endTime, int? reminderMinutes);
        Task DeleteAsync(int id);
        Task<List<CalendarEvent>> GetDueRemindersAsync(DateTime now);
        Task<bool> DismissReminderAsync(int id);
    }
}
