using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Sudan_Train.Service.Models;
using System.Text;
using System.Text.Json;

namespace Sudan_Train.Service.Implementations
{
    public class EmailConsumerService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<EmailConsumerService> _logger;
        private readonly RabbitMQSettings _settings;
        private IConnection? _connection;
        private IChannel? _channel;

        public EmailConsumerService(
            IServiceScopeFactory serviceScopeFactory,
            ILogger<EmailConsumerService> logger,
            IOptions<RabbitMQSettings> settings)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
            _settings = settings.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Email Consumer Service starting...");

            // Retry loop for connection resilience
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var factory = new ConnectionFactory
                    {
                        HostName = _settings.HostName,
                        Port = _settings.Port,
                        UserName = _settings.UserName,
                        Password = _settings.Password,
                        VirtualHost = _settings.VirtualHost
                    };

                    _connection = await factory.CreateConnectionAsync(stoppingToken);
                    _channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);

                    // Connection successful, break retry loop
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to connect to RabbitMQ at {HostName}:{Port}. Retrying in 30 seconds...",
                        _settings.HostName, _settings.Port);

                    try
                    {
                        await Task.Delay(30000, stoppingToken);
                    }
                    catch (TaskCanceledException)
                    {
                        _logger.LogInformation("Email Consumer Service startup cancelled");
                        return;
                    }
                }
            }

            if (stoppingToken.IsCancellationRequested)
                return;

            try
            {

                // Declare queue (in case it doesn't exist yet)
                await _channel!.QueueDeclareAsync(
                    queue: _settings.EmailQueueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null,
                    cancellationToken: stoppingToken);

                // Set prefetch count to process one message at a time
                await _channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false, cancellationToken: stoppingToken);

                var consumer = new AsyncEventingBasicConsumer(_channel);
                consumer.ReceivedAsync += async (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var messageJson = Encoding.UTF8.GetString(body);

                    try
                    {
                        var emailMessage = JsonSerializer.Deserialize<EmailMessage>(messageJson);

                        if (emailMessage != null)
                        {
                            _logger.LogInformation($"Processing queued email to: {emailMessage.To}");

                            // Create a scope to resolve scoped services
                            using var scope = _serviceScopeFactory.CreateScope();

                            // Send email directly using MailKit
                            await SendEmailDirectlyAsync(emailMessage, scope);

                            _logger.LogInformation($"Successfully processed queued email to: {emailMessage.To}");
                        }

                        // Acknowledge message regardless of success/failure (no retry per requirements)
                        await _channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Failed to process queued email. Message: {messageJson}");

                        // Acknowledge message to remove it from queue (no retry per requirements)
                        await _channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
                    }
                };

                await _channel.BasicConsumeAsync(
                    queue: _settings.EmailQueueName,
                    autoAck: false,
                    consumer: consumer,
                    cancellationToken: stoppingToken);

                _logger.LogInformation($"Email Consumer Service started. Listening on queue: {_settings.EmailQueueName}");

                // Keep the service running
                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(1000, stoppingToken);
                }
            }
            catch (TaskCanceledException)
            {
                _logger.LogInformation("Email Consumer Service is shutting down");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Email Consumer Service encountered an error");
                throw;
            }
        }

        private async Task SendEmailDirectlyAsync(EmailMessage emailMessage, IServiceScope scope)
        {
            try
            {
                var emailSettings = scope.ServiceProvider.GetRequiredService<IOptions<EmailSettings>>().Value;

                using var smtpClient = new MailKit.Net.Smtp.SmtpClient();

                await smtpClient.ConnectAsync(emailSettings.Host, emailSettings.Port,
                    emailSettings.EnableSsl ? MailKit.Security.SecureSocketOptions.StartTls : MailKit.Security.SecureSocketOptions.None);

                await smtpClient.AuthenticateAsync(emailSettings.UserName, emailSettings.Password);

                var mimeMessage = new MimeKit.MimeMessage();
                mimeMessage.From.Add(new MimeKit.MailboxAddress(emailSettings.FromName, emailSettings.FromEmail));
                mimeMessage.To.Add(MimeKit.MailboxAddress.Parse(emailMessage.To));
                mimeMessage.Subject = emailMessage.Subject;

                var bodyBuilder = new MimeKit.BodyBuilder
                {
                    HtmlBody = emailMessage.Body,
                    TextBody = emailMessage.Body
                };
                mimeMessage.Body = bodyBuilder.ToMessageBody();

                await smtpClient.SendAsync(mimeMessage);
                await smtpClient.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send queued email to: {emailMessage.To}");
                throw;
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Email Consumer Service stopping...");

            if (_channel != null)
            {
                await _channel.CloseAsync(cancellationToken);
                await _channel.DisposeAsync();
            }

            if (_connection != null)
            {
                await _connection.CloseAsync(cancellationToken);
                await _connection.DisposeAsync();
            }

            await base.StopAsync(cancellationToken);
        }

        public override void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
            base.Dispose();
        }
    }
}
