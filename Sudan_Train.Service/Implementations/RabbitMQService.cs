using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Sudan_Train.Service.Abstracts;
using Sudan_Train.Service.Models;
using System.Text;
using System.Text.Json;

namespace Sudan_Train.Service.Implementations
{
    public class RabbitMQService : IMessageQueueService, IAsyncDisposable
    {
        private readonly RabbitMQSettings _settings;
        private readonly ILogger<RabbitMQService> _logger;
        private IConnection? _connection;
        private IChannel? _channel;
        private readonly SemaphoreSlim _initLock = new SemaphoreSlim(1, 1);
        private bool _initialized = false;

        public RabbitMQService(IOptions<RabbitMQSettings> settings, ILogger<RabbitMQService> logger)
        {
            _settings = settings.Value;
            _logger = logger;
        }

        private async Task EnsureInitializedAsync()
        {
            if (_initialized)
                return;

            await _initLock.WaitAsync();
            try
            {
                if (_initialized)
                    return;

                var factory = new ConnectionFactory
                {
                    HostName = _settings.HostName,
                    Port = _settings.Port,
                    UserName = _settings.UserName,
                    Password = _settings.Password,
                    VirtualHost = _settings.VirtualHost
                };

                _connection = await factory.CreateConnectionAsync();
                _channel = await _connection.CreateChannelAsync();

                // Declare queue as durable for persistence
                await _channel.QueueDeclareAsync(
                    queue: _settings.EmailQueueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                _initialized = true;
                _logger.LogInformation($"RabbitMQ connection established. Queue: {_settings.EmailQueueName}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to establish RabbitMQ connection");
                throw;
            }
            finally
            {
                _initLock.Release();
            }
        }

        public async Task QueueEmailAsync(EmailMessage emailMessage)
        {
            try
            {
                await EnsureInitializedAsync();

                emailMessage.QueuedAt = DateTime.UtcNow;

                var messageJson = JsonSerializer.Serialize(emailMessage);
                var body = Encoding.UTF8.GetBytes(messageJson);

                var properties = new BasicProperties
                {
                    Persistent = true // Make message persistent
                };

                await _channel!.BasicPublishAsync(
                    exchange: "",
                    routingKey: _settings.EmailQueueName,
                    mandatory: false,
                    basicProperties: properties,
                    body: body);

                _logger.LogInformation($"Email queued successfully to: {emailMessage.To}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to queue email to: {emailMessage.To}");
                throw;
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_channel != null)
            {
                await _channel.CloseAsync();
                await _channel.DisposeAsync();
            }

            if (_connection != null)
            {
                await _connection.CloseAsync();
                await _connection.DisposeAsync();
            }

            _initLock?.Dispose();
        }
    }
}
