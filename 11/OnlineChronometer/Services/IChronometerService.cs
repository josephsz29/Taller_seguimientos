using OnlineChronometer.Models;

namespace OnlineChronometer.Services
{
    public interface IChronometerService
    {
        Task<List<ChronometerSummary>> GetSummariesAsync();
        Task<Chronometer> CreateAsync(string? name);
        Task DeleteAsync(Guid id);

        Task<ChronometerState?> GetStateAsync(Guid id);
        Task<ChronometerState?> StartAsync(Guid id);
        Task<ChronometerState?> PauseAsync(Guid id);
        Task<ChronometerState?> ResetAsync(Guid id);
        Task<ChronometerState?> LapAsync(Guid id);
    }
}
