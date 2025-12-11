namespace Sudan_Train.MessagingApi.Models.Enums
{
    public enum SendingStrategy
    {
        Direct,    // Send immediately
        Queued,    // Queue to RabbitMQ immediately
        Fallback   // Try direct, queue if fails
    }
}
