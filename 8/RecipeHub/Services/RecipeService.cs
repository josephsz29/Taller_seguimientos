using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using RecipeHub.Data;
using RecipeHub.Models;

namespace RecipeHub.Services
{
    public class RecipeService : IRecipeService
    {
        // Sin caracteres ambiguos (I, O, 0, 1) para que el código sea fácil de leer
        private const string CodeAlphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        private const int CodeLength = 8;

        private readonly AppDbContext _context;

        public RecipeService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Recipe>> SearchAsync(RecipeFilter filter)
        {
            IQueryable<Recipe> query = _context.Recipes.Include(r => r.FoodType);

            // Texto libre: cada palabra debe aparecer en el nombre o en la descripción
            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                var terms = filter.Search.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                foreach (var term in terms)
                {
                    query = query.Where(r =>
                        r.Title.Contains(term) ||
                        (r.Description != null && r.Description.Contains(term)));
                }
            }

            // Tipo de comida
            if (filter.FoodTypeId.HasValue)
            {
                query = query.Where(r => r.FoodTypeId == filter.FoodTypeId.Value);
            }

            // Tiempo máximo de preparación
            if (filter.MaxPrepTime.HasValue)
            {
                query = query.Where(r => r.PrepTimeMinutes <= filter.MaxPrepTime.Value);
            }

            // Solo favoritas
            if (filter.OnlyFavorites)
            {
                query = query.Where(r => r.IsFavorite);
            }

            // Ingredientes separados por coma: la receta debe tener TODOS (cada uno en alguna de sus líneas)
            if (!string.IsNullOrWhiteSpace(filter.Ingredients))
            {
                var ingredientTerms = filter.Ingredients.Split(
                    new[] { ',', ';' },
                    StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                foreach (var term in ingredientTerms)
                {
                    query = query.Where(r => r.Ingredients.Any(i => i.Description.Contains(term)));
                }
            }

            return await query
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<Recipe?> GetByIdAsync(int id)
        {
            return await _context.Recipes
                .Include(r => r.FoodType)
                .Include(r => r.Ingredients)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<Recipe?> GetByShareCodeAsync(string code)
        {
            var normalizedCode = code.Trim().ToUpperInvariant();

            return await _context.Recipes
                .Include(r => r.FoodType)
                .Include(r => r.Ingredients)
                .FirstOrDefaultAsync(r => r.ShareCode == normalizedCode);
        }

        public async Task AddAsync(Recipe recipe)
        {
            recipe.ShareCode = await GenerateUniqueCodeAsync();
            recipe.CreatedAt = DateTime.Now;

            // Al guardar la receta, EF guarda también los ingredientes que están en su lista
            _context.Recipes.Add(recipe);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(Recipe changes)
        {
            var recipe = await _context.Recipes
                .Include(r => r.Ingredients)
                .FirstOrDefaultAsync(r => r.Id == changes.Id);

            if (recipe == null)
            {
                return false;
            }

            recipe.Title = changes.Title;
            recipe.Description = changes.Description;
            recipe.FoodTypeId = changes.FoodTypeId;
            recipe.PrepTimeMinutes = changes.PrepTimeMinutes;
            recipe.Servings = changes.Servings;
            recipe.Instructions = changes.Instructions;

            // Reemplazamos los ingredientes: borramos los anteriores y agregamos los nuevos
            _context.RecipeIngredients.RemoveRange(recipe.Ingredients.ToList());

            foreach (var ingredient in changes.Ingredients)
            {
                _context.RecipeIngredients.Add(new RecipeIngredient
                {
                    RecipeId = recipe.Id,
                    Description = ingredient.Description
                });
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task DeleteAsync(int id)
        {
            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe != null)
            {
                _context.Recipes.Remove(recipe);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ToggleFavoriteAsync(int id)
        {
            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe == null)
            {
                return false;
            }

            recipe.IsFavorite = !recipe.IsFavorite;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<FoodType>> GetFoodTypesAsync()
        {
            return await _context.FoodTypes
                .OrderBy(f => f.Id)
                .ToListAsync();
        }

        private async Task<string> GenerateUniqueCodeAsync()
        {
            while (true)
            {
                var chars = new char[CodeLength];
                for (int i = 0; i < CodeLength; i++)
                {
                    // RandomNumberGenerator es criptográficamente seguro (el código no debe ser adivinable)
                    chars[i] = CodeAlphabet[RandomNumberGenerator.GetInt32(CodeAlphabet.Length)];
                }

                var code = new string(chars);

                if (!await _context.Recipes.AnyAsync(r => r.ShareCode == code))
                {
                    return code;
                }
            }
        }
    }
}
