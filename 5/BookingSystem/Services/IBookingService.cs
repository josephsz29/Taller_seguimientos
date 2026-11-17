using BookingSystem.Models;

namespace BookingSystem.Services
{
    public interface IBookingService
    {
        Task<List<BookableService>> GetServicesAsync();
        Task<BookableService?> GetServiceAsync(int id);
        Task<List<TimeSlot>> GetAvailableSlotsAsync(int serviceId, DateTime date);
        Task<bool> IsSlotAvailableAsync(int serviceId, DateTime startTime);
        Task<Booking?> CreatePendingAsync(int serviceId, DateTime startTime, string customerName, string customerEmail, string? customerPhone);
        Task<Booking?> GetByCodeAsync(string code);
        Task<bool> ConfirmAsync(string code);
        Task<bool> CancelAsync(string code);
    }
}
