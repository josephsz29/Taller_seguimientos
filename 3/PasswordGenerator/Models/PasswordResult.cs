namespace PasswordGenerator.Models
{
    public class PasswordResult
    {
        public string Password { get; set; } = string.Empty;
        public PasswordStrength Strength { get; set; }
        public double EntropyBits { get; set; }
    }
}
