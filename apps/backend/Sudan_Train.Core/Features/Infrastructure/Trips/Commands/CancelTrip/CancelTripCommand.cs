using MediatR;
using Sudan_Train.Core.Bases;

namespace Sudan_Train.Core.Features.Infrastructure.Trips.Commands.CancelTrip
{
    public class CancelTripCommand : IRequest<Response<string>>
    {
        public int Id { get; set; }
        public string? Reason { get; set; }
    }
}
