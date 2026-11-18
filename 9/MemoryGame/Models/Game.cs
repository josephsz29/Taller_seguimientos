using System.ComponentModel.DataAnnotations;

namespace MemoryGame.Models
{
    // Una partida. El servidor guarda aquí TODO el estado, así el jugador no puede hacer trampa
    // (las cartas no reveladas nunca se envían al navegador).
    public class Game
    {
        // Identificador aleatorio e imposible de adivinar (no un número consecutivo)
        public Guid Id { get; set; }

        public Difficulty Difficulty { get; set; }

        // Símbolo de cada carta, ya mezclado y separado por comas. Ejemplo: "3,1,0,3,1,0"
        [Required]
        [StringLength(100)]
        public string Layout { get; set; } = string.Empty;

        // Un carácter por carta: '1' si ya está emparejada, '0' si no. Ejemplo: "001100"
        [Required]
        [StringLength(32)]
        public string MatchedCards { get; set; } = string.Empty;

        // Primera carta volteada que espera a su segunda carta (null si no hay ninguna)
        public int? PendingCardIndex { get; set; }

        public int Moves { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // El tiempo empieza a contar con la primera carta que se voltea
        public DateTime? StartedAt { get; set; }
        public DateTime? FinishedAt { get; set; }

        public int? Score { get; set; }

        // Solo tiene valor si el jugador guardó su puntaje en el ranking
        [StringLength(30)]
        public string? PlayerName { get; set; }

        // Propiedades calculadas (solo lectura): EF no las guarda en la base de datos
        public int[] SymbolIds => Layout.Split(',').Select(s => int.Parse(s)).ToArray();

        public int? DurationSeconds =>
            StartedAt.HasValue && FinishedAt.HasValue
                ? (int)Math.Round((FinishedAt.Value - StartedAt.Value).TotalSeconds)
                : null;

        public string DurationLabel =>
            DurationSeconds.HasValue
                ? $"{DurationSeconds.Value / 60:D2}:{DurationSeconds.Value % 60:D2}"
                : "--:--";
    }
}
