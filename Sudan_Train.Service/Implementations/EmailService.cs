using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using Sudan_Train.Service.Abstracts;
using Sudan_Train.Service.Models;

namespace Sudan_Train.Service.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<EmailSettings> emailSettings, ILogger<EmailService> logger)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            try
            {
                var emailMessage = new MimeMessage();

                // From
                emailMessage.From.Add(new MailboxAddress(_emailSettings.FromName, _emailSettings.FromEmail));

                // To
                emailMessage.To.Add(MailboxAddress.Parse(email));

                // Subject
                emailMessage.Subject = subject;

                // Body
                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = message,
                    TextBody = message
                };
                emailMessage.Body = bodyBuilder.ToMessageBody();

                // Send email
                using var smtpClient = new SmtpClient();

                await smtpClient.ConnectAsync(_emailSettings.Host, _emailSettings.Port,
                    _emailSettings.EnableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None);

                await smtpClient.AuthenticateAsync(_emailSettings.UserName, _emailSettings.Password);

                await smtpClient.SendAsync(emailMessage);

                await smtpClient.DisconnectAsync(true);

                _logger.LogInformation($"Email sent successfully to: {email}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send email to: {email}. Error: {ex.Message}");
                throw;
            }
        }
    }
}
