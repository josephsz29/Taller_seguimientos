using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using PasswordGenerator.Data;
using PasswordGenerator.Models;

namespace PasswordGenerator.Services
{
    public class PasswordService : IPasswordService
    {
        private const string UppercaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string LowercaseChars = "abcdefghijklmnopqrstuvwxyz";
        private const string DigitChars = "0123456789";
        private const string SymbolChars = "!@#$%^&*()-_=+[]{};:,.<>?";
        private const string AmbiguousChars = "Il1O0o";

        private readonly AppDbContext _context;

        public PasswordService(AppDbContext context)
        {
            _context = context;
        }

        public PasswordResult Generate(PasswordOptions options)
        {
            var sets = BuildCharacterSets(options);

            if (sets.Count == 0)
            {
                throw new ArgumentException("Debe seleccionarse al menos un tipo de carácter.");
            }

            if (options.Length < sets.Count)
            {
                throw new ArgumentException("La longitud es menor que la cantidad de tipos seleccionados.");
            }

            var pool = string.Concat(sets);
            var chars = new List<char>(options.Length);

            // 1) Garantizamos al menos un carácter de cada tipo seleccionado
            foreach (var set in sets)
            {
                chars.Add(set[RandomNumberGenerator.GetInt32(set.Length)]);
            }

            // 2) Completamos el resto con caracteres de todo el conjunto
            while (chars.Count < options.Length)
            {
                chars.Add(pool[RandomNumberGenerator.GetInt32(pool.Length)]);
            }

            // 3) Mezclamos (Fisher-Yates) para que los caracteres obligatorios no queden al inicio
            for (int i = chars.Count - 1; i > 0; i--)
            {
                int j = RandomNumberGenerator.GetInt32(i + 1);
                (chars[i], chars[j]) = (chars[j], chars[i]);
            }

            // Entropía = longitud * log2(tamaño del conjunto de caracteres)
            var entropy = options.Length * Math.Log2(pool.Length);

            return new PasswordResult
            {
                Password = new string(chars.ToArray()),
                EntropyBits = entropy,
                Strength = GetStrength(entropy)
            };
        }

        public async Task<PasswordResult> GenerateAndSaveAsync(PasswordOptions options)
        {
            var result = Generate(options);

            // Guardamos solo las opciones usadas, nunca la contraseña
            _context.PasswordRecords.Add(new PasswordRecord
            {
                Length = options.Length,
                IncludeUppercase = options.IncludeUppercase,
                IncludeLowercase = options.IncludeLowercase,
                IncludeDigits = options.IncludeDigits,
                IncludeSymbols = options.IncludeSymbols,
                ExcludeAmbiguous = options.ExcludeAmbiguous,
                Strength = result.Strength
            });
            await _context.SaveChangesAsync();

            return result;
        }

        public async Task<List<PasswordRecord>> GetHistoryAsync(int count = 10)
        {
            return await _context.PasswordRecords
                .OrderByDescending(r => r.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        private static List<string> BuildCharacterSets(PasswordOptions options)
        {
            var sets = new List<string>();

            if (options.IncludeUppercase) sets.Add(UppercaseChars);
            if (options.IncludeLowercase) sets.Add(LowercaseChars);
            if (options.IncludeDigits) sets.Add(DigitChars);
            if (options.IncludeSymbols) sets.Add(SymbolChars);

            if (options.ExcludeAmbiguous)
            {
                // Quitamos los caracteres que se confunden fácilmente al leerlos
                sets = sets
                    .Select(s => new string(s.Where(c => !AmbiguousChars.Contains(c)).ToArray()))
                    .ToList();
            }

            return sets;
        }

        private static PasswordStrength GetStrength(double entropyBits)
        {
            if (entropyBits < 40) return PasswordStrength.Weak;
            if (entropyBits < 70) return PasswordStrength.Medium;
            return PasswordStrength.Strong;
        }
    }
}
