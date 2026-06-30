using MediatR;
using Sudan_Train.Core.Bases;

namespace Sudan_Train.Core.Features.Bookings.Commands.ReleaseSeatHolds
{
    public class ReleaseSeatHoldsCommand : IRequest<Response<string>>
    {
        public Guid? HoldGroupId { get; set; }
    }
}
