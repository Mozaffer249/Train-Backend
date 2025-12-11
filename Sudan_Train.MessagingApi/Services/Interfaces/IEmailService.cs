using Sudan_Train.MessagingApi.Models.Enums;

namespace Sudan_Train.MessagingApi.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string email, string subject, string message);
        Task SendEmailAsync(string email, string subject, string message, SendingStrategy strategy);
    }
}
