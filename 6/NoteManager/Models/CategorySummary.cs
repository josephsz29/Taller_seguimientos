namespace NoteManager.Models
{
    // Categoría con la cantidad de notas que tiene (no se guarda en la base de datos)
    public class CategorySummary
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int NoteCount { get; set; }
    }
}
