using System.ComponentModel.DataAnnotations;

namespace SurveyPlatform.Models
{
    // Datos del formulario para agregar una pregunta
    public class QuestionFormViewModel : IValidatableObject
    {
        private const int MaxOptions = 10;
        private const int MaxOptionLength = 150;

        public int SurveyId { get; set; }

        [Required(ErrorMessage = "Escribe la pregunta")]
        [StringLength(300, ErrorMessage = "La pregunta no puede superar 300 caracteres")]
        [Display(Name = "Pregunta")]
        public string Text { get; set; } = string.Empty;

        [Display(Name = "Tipo de pregunta")]
        public QuestionType Type { get; set; } = QuestionType.SingleChoice;

        [Display(Name = "Respuesta obligatoria")]
        public bool IsRequired { get; set; }

        [Display(Name = "Opciones (una por línea)")]
        public string? OptionsText { get; set; }

        // Convierte el texto del formulario en una lista de opciones (una línea = una opción)
        public List<string> GetOptionLines()
        {
            return (OptionsText ?? string.Empty)
                .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .ToList();
        }

        // Validaciones que dependen del tipo de pregunta
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!Type.HasOptions())
            {
                yield break;
            }

            var lines = GetOptionLines();

            if (lines.Count < 2)
            {
                yield return new ValidationResult(
                    "Escribe al menos 2 opciones (una por línea).",
                    new[] { nameof(OptionsText) });
            }
            else if (lines.Count > MaxOptions)
            {
                yield return new ValidationResult(
                    "Máximo " + MaxOptions + " opciones por pregunta.",
                    new[] { nameof(OptionsText) });
            }
            else if (lines.Any(line => line.Length > MaxOptionLength))
            {
                yield return new ValidationResult(
                    "Cada opción puede tener máximo " + MaxOptionLength + " caracteres.",
                    new[] { nameof(OptionsText) });
            }
            else if (lines.Distinct(StringComparer.OrdinalIgnoreCase).Count() != lines.Count)
            {
                yield return new ValidationResult(
                    "Hay opciones repetidas.",
                    new[] { nameof(OptionsText) });
            }
        }
    }
}
