using Sudan_Train.MessagingApi.Models.Enums;

namespace Sudan_Train.MessagingApi.Services.Interfaces
{
    public interface ISmsService
    {
        Task SendSmsAsync(string phoneNumber, string content);
        Task SendSmsAsync(string phoneNumber, string content, SendingStrategy strategy);
    }
}
