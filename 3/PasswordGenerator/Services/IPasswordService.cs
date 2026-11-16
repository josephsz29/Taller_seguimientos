using PasswordGenerator.Models;

namespace PasswordGenerator.Services
{
    public interface IPasswordService
    {
        PasswordResult Generate(PasswordOptions options);
        Task<PasswordResult> GenerateAndSaveAsync(PasswordOptions options);
        Task<List<PasswordRecord>> GetHistoryAsync(int count = 10);
    }
}
