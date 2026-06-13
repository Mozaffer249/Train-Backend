using MediatR;
using Sudan_Train.Core.Bases;

namespace Sudan_Train.Core.Features.Boarding.Commands.MarkNoShow
{
    public class MarkNoShowCommand : IRequest<Response<string>>
    {
        public int TicketId { get; set; }
    }
}
