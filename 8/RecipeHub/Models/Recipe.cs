using System.ComponentModel.DataAnnotations;

namespace RecipeHub.Models
{
    public class Recipe
    {
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Title { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        // Sin [StringLength]: en SQL Server se guarda como nvarchar(max)
        [Required]
        public string Instructions { get; set; } = string.Empty;

        public int PrepTimeMinutes { get; set; }
        public int Servings { get; set; }

        // Llave foránea y propiedad de navegación hacia el tipo de comida
        public int FoodTypeId { get; set; }
        public FoodType FoodType { get; set; } = null!;

        // Receta guardada en favoritas
        public bool IsFavorite { get; set; }

        // Código aleatorio para el enlace de compartir (no se exponen los Id consecutivos)
        [Required]
        [StringLength(10)]
        public string ShareCode { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Una receta tiene muchos ingredientes
        public List<RecipeIngredient> Ingredients { get; set; } = new();
    }
}
