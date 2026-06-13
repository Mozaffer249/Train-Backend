using MediatR;
using Sudan_Train.Core.Bases;

namespace Sudan_Train.Core.Features.Infrastructure.Trips.Commands.MarkTripArrived
{
    public class MarkTripArrivedCommand : IRequest<Response<string>>
    {
        public int Id { get; set; }
    }
}
