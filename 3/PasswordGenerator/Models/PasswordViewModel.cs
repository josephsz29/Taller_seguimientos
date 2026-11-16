using System.ComponentModel.DataAnnotations;

namespace PasswordGenerator.Models
{
    public class PasswordViewModel : IValidatableObject
    {
        [Range(4, 64, ErrorMessage = "La longitud debe estar entre 4 y 64")]
        [Display(Name = "Longitud")]
        public int Length { get; set; } = 16;

        [Display(Name = "Mayúsculas (A-Z)")]
        public bool IncludeUppercase { get; set; } = true;

        [Display(Name = "Minúsculas (a-z)")]
        public bool IncludeLowercase { get; set; } = true;

        [Display(Name = "Números (0-9)")]
        public bool IncludeDigits { get; set; } = true;

        [Display(Name = "Símbolos (!@#$...)")]
        public bool IncludeSymbols { get; set; } = true;

        [Display(Name = "Excluir caracteres ambiguos (l, 1, I, O, 0)")]
        public bool ExcludeAmbiguous { get; set; }

        // Es null hasta que se genera una contraseña
        public PasswordResult? Result { get; set; }

        // Últimas generaciones guardadas en la base de datos
        public List<PasswordRecord> History { get; set; } = new();

        // Validación a nivel de todo el formulario: debe haber al menos un tipo de carácter
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!IncludeUppercase && !IncludeLowercase && !IncludeDigits && !IncludeSymbols)
            {
                yield return new ValidationResult("Selecciona al menos un tipo de carácter.");
            }
        }

        public PasswordOptions ToOptions()
        {
            return new PasswordOptions
            {
                Length = Length,
                IncludeUppercase = IncludeUppercase,
                IncludeLowercase = IncludeLowercase,
                IncludeDigits = IncludeDigits,
                IncludeSymbols = IncludeSymbols,
                ExcludeAmbiguous = ExcludeAmbiguous
            };
        }
    }
}
