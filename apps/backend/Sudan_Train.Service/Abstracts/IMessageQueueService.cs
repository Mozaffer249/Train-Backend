using Sudan_Train.Service.Models;

namespace Sudan_Train.Service.Abstracts
{
    public interface IMessageQueueService
    {
        Task QueueEmailAsync(EmailMessage emailMessage);
    }
}
