namespace MemoryGame.Models
{
    // Respuesta (en JSON) cuando el jugador voltea una carta
    public class FlipResult
    {
        public bool Success { get; set; }
        public string? Error { get; set; }

        public int CardIndex { get; set; }
        public string Symbol { get; set; } = string.Empty;

        // true si esta fue la segunda carta del intento
        public bool IsSecondFlip { get; set; }
        public int FirstCardIndex { get; set; }
        public bool IsMatch { get; set; }

        public int Moves { get; set; }
        public int MatchedPairs { get; set; }

        public bool IsFinished { get; set; }
        public int? Score { get; set; }
        public string? DurationLabel { get; set; }

        public static FlipResult Fail(string error)
        {
            return new FlipResult { Success = false, Error = error };
        }
    }
}
