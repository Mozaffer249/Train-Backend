using Sudan_Train.MessagingApi.Models.Enums;

namespace Sudan_Train.MessagingApi.Models.Responses
{
    public class MessageResponse
    {
        public string MessageId { get; set; } = string.Empty;
        public MessageType Type { get; set; }
        public MessageStatus Status { get; set; }
        public DateTime QueuedAt { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool Success { get; set; }
    }
}
