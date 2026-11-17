namespace PasswordGenerator.Models
{
    // Historial de generaciones. Por seguridad no se guarda la contraseña,
    // solo las opciones con las que se generó.
    public class PasswordRecord
    {
        public int Id { get; set; }
        public int Length { get; set; }
        public bool IncludeUppercase { get; set; }
        public bool IncludeLowercase { get; set; }
        public bool IncludeDigits { get; set; }
        public bool IncludeSymbols { get; set; }
        public bool ExcludeAmbiguous { get; set; }
        public PasswordStrength Strength { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
