using Sudan_Train.Service.Models;

namespace Sudan_Train.Service.Abstracts
{
    public interface IEmailService
    {
        Task SendEmailAsync(string email, string subject, string message);
        Task SendEmailAsync(string email, string subject, string message, EmailSendingStrategy strategy);
    }
}
