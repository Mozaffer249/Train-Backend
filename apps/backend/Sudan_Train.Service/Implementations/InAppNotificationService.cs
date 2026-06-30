using Sudan_Train.Data.Entity;
using Sudan_Train.Infrastructure.context;
using Sudan_Train.Service.Abstracts;

namespace Sudan_Train.Service.Implementations
{
    public class InAppNotificationService : IInAppNotificationService
    {
        private readonly ApplicationDBContext _db;

        public InAppNotificationService(ApplicationDBContext db)
        {
            _db = db;
        }

        public async Task NotifyAsync(
            int userId,
            NotificationType type,
            string subject,
            string message,
            int? bookingId = null,
            NotificationChannel channel = NotificationChannel.InApp)
        {
            var now = DateTime.UtcNow;
            _db.Notifications.Add(new Notification
            {
                UserId = userId,
                BookingId = bookingId,
                Type = type,
                Channel = channel,
                Subject = subject,
                Message = message,
                IsRead = false,
                IsSent = true,
                SentAt = now,
                CreatedAt = now,
            });
            await _db.SaveChangesAsync();
        }
    }
}
