namespace PasswordGenerator.Models
{
    // Opciones que recibe el servicio para generar la contraseña
    public class PasswordOptions
    {
        public int Length { get; set; } = 16;
        public bool IncludeUppercase { get; set; } = true;
        public bool IncludeLowercase { get; set; } = true;
        public bool IncludeDigits { get; set; } = true;
        public bool IncludeSymbols { get; set; } = true;
        public bool ExcludeAmbiguous { get; set; }
    }
}
