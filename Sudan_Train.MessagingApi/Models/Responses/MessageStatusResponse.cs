using Sudan_Train.MessagingApi.Models.Enums;

namespace Sudan_Train.MessagingApi.Models.Responses
{
    public class MessageStatusResponse
    {
        public string MessageId { get; set; } = string.Empty;
        public MessageType Type { get; set; }
        public MessageStatus Status { get; set; }
        public DateTime QueuedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public string? ErrorMessage { get; set; }
        public int RetryCount { get; set; }
    }
}
