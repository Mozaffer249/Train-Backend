using MediatR;
using Sudan_Train.Core.Bases;

namespace Sudan_Train.Core.Features.Notifications.Commands.MarkNotificationRead
{
    public class MarkNotificationReadCommand : IRequest<Response<string>>
    {
        public int Id { get; set; }
    }
}
