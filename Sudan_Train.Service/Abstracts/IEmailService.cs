namespace Sudan_Train.Service.Abstracts
{
    public interface IEmailService
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
