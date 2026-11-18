using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RecipeHub.Models;
using RecipeHub.Services;

namespace RecipeHub.Controllers
{
    public class RecipesController : Controller
    {
        private readonly IRecipeService _recipeService;

        public RecipesController(IRecipeService recipeService)
        {
            _recipeService = recipeService;
        }

        // GET: /Recipes?Search=pollo&FoodTypeId=2&Ingredients=arroz,ajo  -> búsqueda y filtros
        [HttpGet]
        public async Task<IActionResult> Index(RecipeFilter filter)
        {
            var model = new RecipeIndexViewModel
            {
                Filter = filter,
                Recipes = await _recipeService.SearchAsync(filter),
                FoodTypes = await GetFoodTypeItemsAsync()
            };

            return View(model);
        }

        // GET: /Recipes/Details/5  -> receta completa con opciones para editar y compartir
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var recipe = await _recipeService.GetByIdAsync(id);
            if (recipe == null)
            {
                return NotFound();
            }

            return View(recipe);
        }

        // GET: /Recipes/Shared?code=AB12CD34  -> enlace para compartir (solo lectura)
        [HttpGet]
        public async Task<IActionResult> Shared(string? code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return NotFound();
            }

            var recipe = await _recipeService.GetByShareCodeAsync(code);
            if (recipe == null)
            {
                return NotFound();
            }

            return View(recipe);
        }

        // GET: /Recipes/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new RecipeFormViewModel
            {
                FoodTypes = await GetFoodTypeItemsAsync()
            };

            return View("Form", model);
        }

        // POST: /Recipes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RecipeFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.FoodTypes = await GetFoodTypeItemsAsync();
                return View("Form", model);
            }

            var recipe = BuildRecipe(model);
            await _recipeService.AddAsync(recipe);

            TempData["Success"] = "Receta guardada.";
            return RedirectToAction(nameof(Details), new { id = recipe.Id });
        }

        // GET: /Recipes/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var recipe = await _recipeService.GetByIdAsync(id);
            if (recipe == null)
            {
                return NotFound();
            }

            var model = new RecipeFormViewModel
            {
                Id = recipe.Id,
                Title = recipe.Title,
                Description = recipe.Description,
                FoodTypeId = recipe.FoodTypeId,
                PrepTimeMinutes = recipe.PrepTimeMinutes,
                Servings = recipe.Servings,
                Instructions = recipe.Instructions,
                IngredientsText = string.Join(Environment.NewLine, recipe.Ingredients.OrderBy(i => i.Id).Select(i => i.Description)),
                FoodTypes = await GetFoodTypeItemsAsync()
            };

            return View("Form", model);
        }

        // POST: /Recipes/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(RecipeFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.FoodTypes = await GetFoodTypeItemsAsync();
                return View("Form", model);
            }

            var changes = BuildRecipe(model);
            changes.Id = model.Id;

            var updated = await _recipeService.UpdateAsync(changes);
            if (!updated)
            {
                return NotFound();
            }

            TempData["Success"] = "Receta actualizada.";
            return RedirectToAction(nameof(Details), new { id = model.Id });
        }

        // POST: /Recipes/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _recipeService.DeleteAsync(id);

            TempData["Success"] = "Receta eliminada.";
            return RedirectToAction(nameof(Index));
        }

        // POST: /Recipes/ToggleFavorite/5  -> guarda o quita de favoritas
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleFavorite(int id, string? returnUrl)
        {
            await _recipeService.ToggleFavoriteAsync(id);

            // Volvemos a la página donde estaba el usuario (solo si es una dirección de este mismo sitio)
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return LocalRedirect(returnUrl);
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Error()
        {
            return Content("Ocurrió un error inesperado.");
        }

        // Convierte el formulario en una receta con su lista de ingredientes
        private static Recipe BuildRecipe(RecipeFormViewModel model)
        {
            return new Recipe
            {
                Title = model.Title.Trim(),
                Description = string.IsNullOrWhiteSpace(model.Description) ? null : model.Description.Trim(),
                FoodTypeId = model.FoodTypeId,
                PrepTimeMinutes = model.PrepTimeMinutes,
                Servings = model.Servings,
                Instructions = model.Instructions.Trim(),
                Ingredients = model.GetIngredientLines()
                    .Select(line => new RecipeIngredient { Description = line })
                    .ToList()
            };
        }

        private async Task<List<SelectListItem>> GetFoodTypeItemsAsync()
        {
            var foodTypes = await _recipeService.GetFoodTypesAsync();

            return foodTypes
                .Select(f => new SelectListItem(f.Name, f.Id.ToString()))
                .ToList();
        }
    }
}
