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
        private readonly IMessageQueueService _messageQueueService;

        public EmailService(
            IOptions<EmailSettings> emailSettings,
            ILogger<EmailService> logger,
            IMessageQueueService messageQueueService)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;
            _messageQueueService = messageQueueService;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            await SendEmailAsync(email, subject, message, _emailSettings.DefaultStrategy);
        }

        public async Task SendEmailAsync(string email, string subject, string message, EmailSendingStrategy strategy)
        {
            switch (strategy)
            {
                case EmailSendingStrategy.Direct:
                    await SendDirectAsync(email, subject, message);
                    break;

                case EmailSendingStrategy.Queued:
                    await QueueEmailAsync(email, subject, message);
                    break;

                case EmailSendingStrategy.Fallback:
                    await SendWithFallbackAsync(email, subject, message);
                    break;

                default:
                    throw new ArgumentException($"Unknown email sending strategy: {strategy}");
            }
        }

        private async Task SendDirectAsync(string email, string subject, string message)
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

            _logger.LogInformation($"Email sent successfully (Direct) to: {email}");
        }

        private async Task QueueEmailAsync(string email, string subject, string message)
        {
            await _messageQueueService.QueueEmailAsync(new EmailMessage
            {
                To = email,
                Subject = subject,
                Body = message
            });

            _logger.LogInformation($"Email queued for delivery to: {email}");
        }

        private async Task SendWithFallbackAsync(string email, string subject, string message)
        {
            try
            {
                await SendDirectAsync(email, subject, message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send email to: {email}. Error: {ex.Message}");

                // Fallback: Queue the email for later processing
                try
                {
                    await QueueEmailAsync(email, subject, message);
                    _logger.LogInformation($"Email queued for later delivery (Fallback) to: {email}");
                }
                catch (Exception queueEx)
                {
                    _logger.LogError(queueEx, $"Failed to queue email to: {email}. Email will be lost.");
                    throw;
                }
            }
        }
    }
}
