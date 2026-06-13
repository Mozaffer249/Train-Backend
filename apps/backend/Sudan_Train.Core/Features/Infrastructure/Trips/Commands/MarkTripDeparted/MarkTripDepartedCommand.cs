using MediatR;
using Sudan_Train.Core.Bases;

namespace Sudan_Train.Core.Features.Infrastructure.Trips.Commands.MarkTripDeparted
{
    public class MarkTripDepartedCommand : IRequest<Response<string>>
    {
        public int Id { get; set; }
    }
}
