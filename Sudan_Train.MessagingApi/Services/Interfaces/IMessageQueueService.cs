using Sudan_Train.MessagingApi.Models.Entities;

namespace Sudan_Train.MessagingApi.Services.Interfaces
{
    public interface IMessageQueueService
    {
        Task QueueEmailAsync(EmailMessage emailMessage);
        Task QueueSmsAsync(SmsMessage smsMessage);
        Task QueuePushNotificationAsync(PushNotificationMessage pushMessage);
    }
}
