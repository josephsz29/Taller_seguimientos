namespace MemoryGame.Models
{
    // Las "imágenes" de las cartas. Cada carta guarda solo el número de su símbolo.
    // Para usar imágenes reales, cambia estos textos por rutas (por ejemplo "/img/perro.png")
    // y muéstralas con <img> en lugar de texto.
    public static class CardSymbols
    {
        public static readonly IReadOnlyList<string> All = new List<string>
        {
            "🐶", "🐱", "🐭", "🐹", "🐰", "🦊",
            "🐻", "🐼", "🐨", "🐯", "🦁", "🐮"
        };

        public static string Get(int id)
        {
            return All[id];
        }
    }
}
