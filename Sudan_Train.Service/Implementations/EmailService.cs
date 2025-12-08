using Microsoft.Extensions.Logging;
using Sudan_Train.Service.Abstracts;

namespace Sudan_Train.Service.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;

        public EmailService(ILogger<EmailService> logger)
        {
            _logger = logger;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            // TODO: Implement actual email sending logic
            // This is a placeholder implementation
            // You can integrate with services like SendGrid, SMTP, or other email providers

            _logger.LogInformation($"Sending email to: {email}");
            _logger.LogInformation($"Subject: {subject}");
            _logger.LogInformation($"Message: {message}");

            // Simulate async operation
            await Task.CompletedTask;

            _logger.LogInformation("Email sent successfully (placeholder)");
        }
    }
}
