namespace MemoryGame.Models
{
    // Datos de la página del juego
    public class GameBoardViewModel
    {
        public Guid GameId { get; set; }
        public Difficulty Difficulty { get; set; }
        public string DifficultyLabel { get; set; } = string.Empty;
        public int Columns { get; set; }
        public int TotalPairs { get; set; }
        public int MatchedPairs { get; set; }
        public int Moves { get; set; }

        public bool IsStarted { get; set; }
        public bool IsFinished { get; set; }
        public int ElapsedSeconds { get; set; }

        public int? Score { get; set; }
        public string DurationLabel { get; set; } = "--:--";

        // Nombre con el que se guardó el puntaje (null si todavía no se guarda)
        public string? PlayerName { get; set; }
        public bool IsSaved => PlayerName != null;

        public List<BoardCard> Cards { get; set; } = new();
    }
}
