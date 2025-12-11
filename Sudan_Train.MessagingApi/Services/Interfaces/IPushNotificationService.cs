using Sudan_Train.MessagingApi.Models.Enums;

namespace Sudan_Train.MessagingApi.Services.Interfaces
{
    public interface IPushNotificationService
    {
        Task SendPushNotificationAsync(string deviceToken, string title, string body, Dictionary<string, object>? data = null);
        Task SendPushNotificationAsync(string deviceToken, string title, string body, SendingStrategy strategy, Dictionary<string, object>? data = null);
    }
}
