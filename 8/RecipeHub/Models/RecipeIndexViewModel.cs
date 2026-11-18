using Microsoft.AspNetCore.Mvc.Rendering;

namespace RecipeHub.Models
{
    // Datos de la página principal: filtros + lista de recetas
    public class RecipeIndexViewModel
    {
        public RecipeFilter Filter { get; set; } = new();
        public List<Recipe> Recipes { get; set; } = new();
        public List<SelectListItem> FoodTypes { get; set; } = new();
    }
}
