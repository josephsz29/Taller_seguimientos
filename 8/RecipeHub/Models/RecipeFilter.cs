namespace RecipeHub.Models
{
    // Filtros de búsqueda; llegan por la URL (por ejemplo /Recipes?Search=pollo&MaxPrepTime=30)
    public class RecipeFilter
    {
        public string? Search { get; set; }
        public int? FoodTypeId { get; set; }

        // Ingredientes separados por coma: la receta debe contener todos
        public string? Ingredients { get; set; }

        public int? MaxPrepTime { get; set; }
        public bool OnlyFavorites { get; set; }

        public bool HasFilters =>
            !string.IsNullOrWhiteSpace(Search) ||
            FoodTypeId.HasValue ||
            !string.IsNullOrWhiteSpace(Ingredients) ||
            MaxPrepTime.HasValue ||
            OnlyFavorites;
    }
}
