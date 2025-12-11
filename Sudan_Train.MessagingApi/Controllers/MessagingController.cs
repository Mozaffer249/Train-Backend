using Microsoft.AspNetCore.Mvc;
using Sudan_Train.MessagingApi.Models.Enums;
using Sudan_Train.MessagingApi.Models.Requests;
using Sudan_Train.MessagingApi.Models.Responses;
using Sudan_Train.MessagingApi.Services.Interfaces;

namespace Sudan_Train.MessagingApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessagingController : ControllerBase
    {
        private readonly IEmailService _emailService;
        private readonly ISmsService _smsService;
        private readonly IPushNotificationService _pushNotificationService;
        private readonly IMessageTrackingService _messageTrackingService;
        private readonly ILogger<MessagingController> _logger;

        public MessagingController(
            IEmailService emailService,
            ISmsService smsService,
            IPushNotificationService pushNotificationService,
            IMessageTrackingService messageTrackingService,
            ILogger<MessagingController> logger)
        {
            _emailService = emailService;
            _smsService = smsService;
            _pushNotificationService = pushNotificationService;
            _messageTrackingService = messageTrackingService;
            _logger = logger;
        }

        /// <summary>
        /// Send a single email
        /// </summary>
        [HttpPost("email")]
        public async Task<ActionResult<MessageResponse>> SendEmail([FromBody] SendEmailRequest request)
        {
            try
            {
                var messageId = Guid.NewGuid().ToString();

                // Log message to database
                // await _messageTrackingService.LogMessageAsync(
                //     messageId,
                //     MessageType.Email,
                //     request.To,
                //     request.Subject,
                //     request.Body);

                // Send email
                await _emailService.SendEmailAsync(
                    request.To,
                    request.Subject,
                    request.Body,
                    request.Strategy);

                return Ok(new MessageResponse
                {
                    MessageId = messageId,
                    Type = MessageType.Email,
                    Status = request.Strategy == SendingStrategy.Direct ? MessageStatus.Sent : MessageStatus.Queued,
                    QueuedAt = DateTime.UtcNow,
                    Message = "Email processed successfully",
                    Success = true
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to: {To}", request.To);
                return StatusCode(500, new MessageResponse
                {
                    MessageId = string.Empty,
                    Type = MessageType.Email,
                    Status = MessageStatus.Failed,
                    Message = $"Failed to send email: {ex.Message}",
                    Success = false
                });
            }
        }

        /// <summary>
        /// Send bulk emails
        /// </summary>
        [HttpPost("email/bulk")]
        public async Task<ActionResult<List<MessageResponse>>> SendBulkEmail([FromBody] List<SendEmailRequest> requests)
        {
            var responses = new List<MessageResponse>();

            foreach (var request in requests)
            {
                try
                {
                    var messageId = Guid.NewGuid().ToString();

                    await _messageTrackingService.LogMessageAsync(
                        messageId,
                        MessageType.Email,
                        request.To,
                        request.Subject,
                        request.Body);

                    await _emailService.SendEmailAsync(
                        request.To,
                        request.Subject,
                        request.Body,
                        request.Strategy);

                    responses.Add(new MessageResponse
                    {
                        MessageId = messageId,
                        Type = MessageType.Email,
                        Status = request.Strategy == SendingStrategy.Direct ? MessageStatus.Sent : MessageStatus.Queued,
                        QueuedAt = DateTime.UtcNow,
                        Message = "Email processed successfully",
                        Success = true
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send email to: {To}", request.To);
                    responses.Add(new MessageResponse
                    {
                        MessageId = string.Empty,
                        Type = MessageType.Email,
                        Status = MessageStatus.Failed,
                        Message = $"Failed: {ex.Message}",
                        Success = false
                    });
                }
            }

            return Ok(responses);
        }

        /// <summary>
        /// Send a single SMS
        /// </summary>
        [HttpPost("sms")]
        public async Task<ActionResult<MessageResponse>> SendSms([FromBody] SendSmsRequest request)
        {
            try
            {
                var messageId = Guid.NewGuid().ToString();

                // Log message to database
                await _messageTrackingService.LogMessageAsync(
                    messageId,
                    MessageType.SMS,
                    request.PhoneNumber,
                    string.Empty,
                    request.Content);

                // Send SMS
                await _smsService.SendSmsAsync(
                    request.PhoneNumber,
                    request.Content,
                    request.Strategy);

                return Ok(new MessageResponse
                {
                    MessageId = messageId,
                    Type = MessageType.SMS,
                    Status = request.Strategy == SendingStrategy.Direct ? MessageStatus.Sent : MessageStatus.Queued,
                    QueuedAt = DateTime.UtcNow,
                    Message = "SMS processed successfully",
                    Success = true
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send SMS to: {PhoneNumber}", request.PhoneNumber);
                return StatusCode(500, new MessageResponse
                {
                    MessageId = string.Empty,
                    Type = MessageType.SMS,
                    Status = MessageStatus.Failed,
                    Message = $"Failed to send SMS: {ex.Message}",
                    Success = false
                });
            }
        }

        /// <summary>
        /// Send bulk SMS
        /// </summary>
        [HttpPost("sms/bulk")]
        public async Task<ActionResult<List<MessageResponse>>> SendBulkSms([FromBody] List<SendSmsRequest> requests)
        {
            var responses = new List<MessageResponse>();

            foreach (var request in requests)
            {
                try
                {
                    var messageId = Guid.NewGuid().ToString();

                    await _messageTrackingService.LogMessageAsync(
                        messageId,
                        MessageType.SMS,
                        request.PhoneNumber,
                        string.Empty,
                        request.Content);

                    await _smsService.SendSmsAsync(
                        request.PhoneNumber,
                        request.Content,
                        request.Strategy);

                    responses.Add(new MessageResponse
                    {
                        MessageId = messageId,
                        Type = MessageType.SMS,
                        Status = request.Strategy == SendingStrategy.Direct ? MessageStatus.Sent : MessageStatus.Queued,
                        QueuedAt = DateTime.UtcNow,
                        Message = "SMS processed successfully",
                        Success = true
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send SMS to: {PhoneNumber}", request.PhoneNumber);
                    responses.Add(new MessageResponse
                    {
                        MessageId = string.Empty,
                        Type = MessageType.SMS,
                        Status = MessageStatus.Failed,
                        Message = $"Failed: {ex.Message}",
                        Success = false
                    });
                }
            }

            return Ok(responses);
        }

        /// <summary>
        /// Send a push notification
        /// </summary>
        [HttpPost("push")]
        public async Task<ActionResult<MessageResponse>> SendPushNotification([FromBody] SendPushNotificationRequest request)
        {
            try
            {
                var messageId = Guid.NewGuid().ToString();

                // Log message to database
                await _messageTrackingService.LogMessageAsync(
                    messageId,
                    MessageType.PushNotification,
                    request.DeviceToken,
                    request.Title,
                    request.Body);

                // Send push notification
                await _pushNotificationService.SendPushNotificationAsync(
                    request.DeviceToken,
                    request.Title,
                    request.Body,
                    request.Strategy,
                    request.Data);

                return Ok(new MessageResponse
                {
                    MessageId = messageId,
                    Type = MessageType.PushNotification,
                    Status = request.Strategy == SendingStrategy.Direct ? MessageStatus.Sent : MessageStatus.Queued,
                    QueuedAt = DateTime.UtcNow,
                    Message = "Push notification processed successfully",
                    Success = true
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send push notification to: {DeviceToken}", request.DeviceToken);
                return StatusCode(500, new MessageResponse
                {
                    MessageId = string.Empty,
                    Type = MessageType.PushNotification,
                    Status = MessageStatus.Failed,
                    Message = $"Failed to send push notification: {ex.Message}",
                    Success = false
                });
            }
        }

        /// <summary>
        /// Send bulk push notifications
        /// </summary>
        [HttpPost("push/bulk")]
        public async Task<ActionResult<List<MessageResponse>>> SendBulkPushNotification([FromBody] List<SendPushNotificationRequest> requests)
        {
            var responses = new List<MessageResponse>();

            foreach (var request in requests)
            {
                try
                {
                    var messageId = Guid.NewGuid().ToString();

                    await _messageTrackingService.LogMessageAsync(
                        messageId,
                        MessageType.PushNotification,
                        request.DeviceToken,
                        request.Title,
                        request.Body);

                    await _pushNotificationService.SendPushNotificationAsync(
                        request.DeviceToken,
                        request.Title,
                        request.Body,
                        request.Strategy,
                        request.Data);

                    responses.Add(new MessageResponse
                    {
                        MessageId = messageId,
                        Type = MessageType.PushNotification,
                        Status = request.Strategy == SendingStrategy.Direct ? MessageStatus.Sent : MessageStatus.Queued,
                        QueuedAt = DateTime.UtcNow,
                        Message = "Push notification processed successfully",
                        Success = true
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send push notification to: {DeviceToken}", request.DeviceToken);
                    responses.Add(new MessageResponse
                    {
                        MessageId = string.Empty,
                        Type = MessageType.PushNotification,
                        Status = MessageStatus.Failed,
                        Message = $"Failed: {ex.Message}",
                        Success = false
                    });
                }
            }

            return Ok(responses);
        }

        /// <summary>
        /// Get message status by ID
        /// </summary>
        [HttpGet("status/{messageId}")]
        public async Task<ActionResult<MessageStatusResponse>> GetMessageStatus(string messageId)
        {
            try
            {
                var messageLog = await _messageTrackingService.GetMessageStatusAsync(messageId);

                if (messageLog == null)
                {
                    return NotFound(new { message = "Message not found" });
                }

                return Ok(new MessageStatusResponse
                {
                    MessageId = messageLog.MessageId,
                    Type = messageLog.Type,
                    Status = messageLog.Status,
                    QueuedAt = messageLog.QueuedAt,
                    ProcessedAt = messageLog.ProcessedAt,
                    DeliveredAt = messageLog.DeliveredAt,
                    ErrorMessage = messageLog.ErrorMessage,
                    RetryCount = messageLog.RetryCount
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get message status for: {MessageId}", messageId);
                return StatusCode(500, new { message = "Failed to retrieve message status" });
            }
        }

        /// <summary>
        /// Health check endpoint
        /// </summary>
        [HttpGet("health")]
        public IActionResult Health()
        {
            return Ok(new
            {
                status = "healthy",
                service = "MessagingApi",
                timestamp = DateTime.UtcNow,
                version = "1.0.0"
            });
        }

        /// <summary>
        /// Get message history (paginated)
        /// </summary>
        [HttpGet("history")]
        public async Task<ActionResult<List<MessageStatusResponse>>> GetHistory(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 50)
        {
            try
            {
                var messageLogs = await _messageTrackingService.GetMessageHistoryAsync(pageNumber, pageSize);

                var responses = messageLogs.Select(log => new MessageStatusResponse
                {
                    MessageId = log.MessageId,
                    Type = log.Type,
                    Status = log.Status,
                    QueuedAt = log.QueuedAt,
                    ProcessedAt = log.ProcessedAt,
                    DeliveredAt = log.DeliveredAt,
                    ErrorMessage = log.ErrorMessage,
                    RetryCount = log.RetryCount
                }).ToList();

                return Ok(responses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get message history");
                return StatusCode(500, new { message = "Failed to retrieve message history" });
            }
        }
    }
}
