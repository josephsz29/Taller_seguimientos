namespace MemoryGame.Models
{
    // Una carta del tablero tal como la ve el navegador
    public class BoardCard
    {
        public int Index { get; set; }
        public bool IsMatched { get; set; }
        public bool IsRevealed { get; set; }

        // Solo tiene valor si la carta está revelada; las demás NO envían su símbolo
        public string? Symbol { get; set; }
    }
}
