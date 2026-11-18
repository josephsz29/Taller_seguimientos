using System.ComponentModel.DataAnnotations;

namespace RecipeHub.Models
{
    public class RecipeIngredient
    {
        public int Id { get; set; }

        // Texto completo del ingrediente, por ejemplo "2 tazas de harina"
        [Required]
        [StringLength(200)]
        public string Description { get; set; } = string.Empty;

        public int RecipeId { get; set; }
        public Recipe Recipe { get; set; } = null!;
    }
}
