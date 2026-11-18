using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace RecipeHub.Models
{
    // Datos del formulario para crear o editar una receta (Id = 0 significa receta nueva)
    public class RecipeFormViewModel : IValidatableObject
    {
        private const int MaxIngredients = 50;
        private const int MaxIngredientLength = 200;

        public int Id { get; set; }

        [Required(ErrorMessage = "Escribe el nombre de la receta")]
        [StringLength(150, ErrorMessage = "El nombre no puede superar 150 caracteres")]
        [Display(Name = "Nombre de la receta")]
        public string Title { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "La descripción no puede superar 500 caracteres")]
        [Display(Name = "Descripción corta (opcional)")]
        public string? Description { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Selecciona un tipo de comida")]
        [Display(Name = "Tipo de comida")]
        public int FoodTypeId { get; set; }

        [Range(1, 1440, ErrorMessage = "El tiempo debe estar entre 1 y 1440 minutos")]
        [Display(Name = "Tiempo de preparación (minutos)")]
        public int PrepTimeMinutes { get; set; } = 30;

        [Range(1, 100, ErrorMessage = "Las porciones deben estar entre 1 y 100")]
        [Display(Name = "Porciones")]
        public int Servings { get; set; } = 4;

        [Required(ErrorMessage = "Escribe al menos un ingrediente")]
        [Display(Name = "Ingredientes (uno por línea)")]
        public string IngredientsText { get; set; } = string.Empty;

        [Required(ErrorMessage = "Escribe las instrucciones de preparación")]
        [Display(Name = "Preparación")]
        public string Instructions { get; set; } = string.Empty;

        // Opciones del <select>; no vienen del formulario
        [BindNever]
        public List<SelectListItem> FoodTypes { get; set; } = new();

        // Convierte el texto del formulario en una lista de ingredientes (una línea = un ingrediente)
        public List<string> GetIngredientLines()
        {
            return IngredientsText
                .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .ToList();
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var lines = GetIngredientLines();

            if (lines.Count > MaxIngredients)
            {
                yield return new ValidationResult(
                    "Máximo " + MaxIngredients + " ingredientes por receta.",
                    new[] { nameof(IngredientsText) });
            }

            if (lines.Any(line => line.Length > MaxIngredientLength))
            {
                yield return new ValidationResult(
                    "Cada ingrediente puede tener máximo " + MaxIngredientLength + " caracteres.",
                    new[] { nameof(IngredientsText) });
            }
        }
    }
}
