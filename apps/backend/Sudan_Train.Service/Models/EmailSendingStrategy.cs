namespace Sudan_Train.Service.Models
{
    public enum EmailSendingStrategy
    {
        Direct,    // Send immediately via SMTP
        Queued,    // Queue to RabbitMQ immediately
        Fallback   // Try direct, queue if fails (current behavior)
    }
}
