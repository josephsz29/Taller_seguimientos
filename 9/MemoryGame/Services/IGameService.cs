using MemoryGame.Models;

namespace MemoryGame.Services
{
    public interface IGameService
    {
        Task<Game> StartAsync(Difficulty difficulty);
        Task<Game?> GetAsync(Guid id);
        Task<FlipResult> FlipAsync(Guid id, int cardIndex);
        Task<bool> SaveScoreAsync(Guid id, string playerName);
        Task<List<Game>> GetLeaderboardAsync(Difficulty difficulty, int count);
    }
}
