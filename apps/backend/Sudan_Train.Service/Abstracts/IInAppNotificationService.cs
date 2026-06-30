using Sudan_Train.Data.Entity;

namespace Sudan_Train.Service.Abstracts
{
    public interface IInAppNotificationService
    {
        Task NotifyAsync(
            int userId,
            NotificationType type,
            string subject,
            string message,
            int? bookingId = null,
            NotificationChannel channel = NotificationChannel.InApp);
    }
}
