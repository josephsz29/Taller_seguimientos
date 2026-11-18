namespace MemoryGame.Models
{
    public class LeaderboardViewModel
    {
        public Difficulty Difficulty { get; set; }
        public List<Game> Entries { get; set; } = new();
    }
}
