using Sudan_Train.Data.Helpers;
using System.Threading.Tasks;

namespace Sudan_Train.Service.Abstracts
{
    public interface IPasswordSecurityService
    {
        Task<bool> IsPasswordInHistoryAsync(int userId, string newPassword, int historyCount = 5);
        Task AddToPasswordHistoryAsync(int userId, string passwordHash);
        Task<PasswordStrength> CheckPasswordStrengthAsync(string password);
        Task<bool> IsCommonPasswordAsync(string password);
        Task<bool> IsPasswordExpiredAsync(int userId);
    }
}
