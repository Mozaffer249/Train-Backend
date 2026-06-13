using MediatR;
using Sudan_Train.Core.Bases;

namespace Sudan_Train.Core.Features.Boarding.Commands.BoardTicket
{
    public class BoardTicketCommand : IRequest<Response<string>>
    {
        public int TicketId { get; set; }
    }
}
