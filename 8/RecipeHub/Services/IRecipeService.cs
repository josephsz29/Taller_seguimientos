using RecipeHub.Models;

namespace RecipeHub.Services
{
    public interface IRecipeService
    {
        Task<List<Recipe>> SearchAsync(RecipeFilter filter);
        Task<Recipe?> GetByIdAsync(int id);
        Task<Recipe?> GetByShareCodeAsync(string code);
        Task AddAsync(Recipe recipe);
        Task<bool> UpdateAsync(Recipe changes);
        Task DeleteAsync(int id);
        Task<bool> ToggleFavoriteAsync(int id);
        Task<List<FoodType>> GetFoodTypesAsync();
    }
}
