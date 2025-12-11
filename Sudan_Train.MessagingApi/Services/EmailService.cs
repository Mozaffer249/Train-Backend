using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using Sudan_Train.MessagingApi.Configuration;
using Sudan_Train.MessagingApi.Models.Entities;
using Sudan_Train.MessagingApi.Models.Enums;
using Sudan_Train.MessagingApi.Services.Interfaces;

namespace Sudan_Train.MessagingApi.Services
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

        public async Task SendEmailAsync(string email, string subject, string message, SendingStrategy strategy)
        {
            switch (strategy)
            {
                case SendingStrategy.Direct:
                    await SendDirectAsync(email, subject, message);
                    break;

                case SendingStrategy.Queued:
                    await QueueEmailAsync(email, subject, message);
                    break;

                case SendingStrategy.Fallback:
                    await SendWithFallbackAsync(email, subject, message);
                    break;

                default:
                    throw new ArgumentException($"Unknown email sending strategy: {strategy}");
            }
        }

        private async Task SendDirectAsync(string email, string subject, string message)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress(_emailSettings.FromName, _emailSettings.FromEmail));
            emailMessage.To.Add(MailboxAddress.Parse(email));
            emailMessage.Subject = subject;

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = message,
                TextBody = message
            };
            emailMessage.Body = bodyBuilder.ToMessageBody();

            using var smtpClient = new SmtpClient();

            await smtpClient.ConnectAsync(_emailSettings.Host, _emailSettings.Port,
                _emailSettings.EnableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None);

            await smtpClient.AuthenticateAsync(_emailSettings.UserName, _emailSettings.Password);
            await smtpClient.SendAsync(emailMessage);
            await smtpClient.DisconnectAsync(true);

            _logger.LogInformation("Email sent successfully (Direct) to: {Email}", email);
        }

        private async Task QueueEmailAsync(string email, string subject, string message)
        {
            await _messageQueueService.QueueEmailAsync(new EmailMessage
            {
                To = email,
                Subject = subject,
                Body = message
            });

            _logger.LogInformation("Email queued for delivery to: {Email}", email);
        }

        private async Task SendWithFallbackAsync(string email, string subject, string message)
        {
            try
            {
                await SendDirectAsync(email, subject, message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to: {Email}. Error: {Message}", email, ex.Message);

                try
                {
                    await QueueEmailAsync(email, subject, message);
                    _logger.LogInformation("Email queued for later delivery (Fallback) to: {Email}", email);
                }
                catch (Exception queueEx)
                {
                    _logger.LogError(queueEx, "Failed to queue email to: {Email}. Email will be lost.", email);
                    throw;
                }
            }
        }
    }
}
