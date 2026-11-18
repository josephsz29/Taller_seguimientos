namespace MemoryGame.Models
{
    public class DifficultySetting
    {
        public Difficulty Difficulty { get; set; }
        public string Label { get; set; } = string.Empty;

        // Cantidad de parejas y de columnas del tablero
        public int Pairs { get; set; }
        public int Columns { get; set; }

        public int TotalCards => Pairs * 2;
    }

    // Configuración de cada nivel de dificultad
    public static class DifficultySettings
    {
        public static readonly IReadOnlyList<DifficultySetting> All = new List<DifficultySetting>
        {
            new DifficultySetting { Difficulty = Difficulty.Easy, Label = "Fácil", Pairs = 6, Columns = 4 },
            new DifficultySetting { Difficulty = Difficulty.Medium, Label = "Medio", Pairs = 8, Columns = 4 },
            new DifficultySetting { Difficulty = Difficulty.Hard, Label = "Difícil", Pairs = 12, Columns = 6 }
        };

        public static DifficultySetting Get(Difficulty difficulty)
        {
            return All.First(s => s.Difficulty == difficulty);
        }
    }
}
