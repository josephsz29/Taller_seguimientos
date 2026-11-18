using System.ComponentModel.DataAnnotations;

namespace RecipeHub.Models
{
    // Tipo de comida: desayuno, almuerzo, postre...
    public class FoodType
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        // Un tipo de comida tiene muchas recetas
        public List<Recipe> Recipes { get; set; } = new();
    }
}
