using Sudan_Train.Data.Entity.Identity;

namespace Sudan_Train.Service.Abstracts
{
    public interface ISecurityNotificationService
    {
        Task NotifyPasswordChangedAsync(User user, string ipAddress);
        Task NotifyEmailChangedAsync(string oldEmail, string newEmail, string userName, string ipAddress);
        Task NotifyNewDeviceLoginAsync(User user, string deviceInfo, string ipAddress);
        Task NotifyTwoFactorEnabledAsync(User user);
        Task NotifyTwoFactorDisabledAsync(User user);
        Task NotifySessionTerminatedAsync(User user, string deviceInfo);
        Task NotifySuspiciousActivityAsync(User user, string activity, string ipAddress);
        Task NotifyAccountDeletedAsync(User user);
    }
}

