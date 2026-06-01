using MediatR;
using Sudan_Train.Core.Bases;
using Sudan_Train.Data.DTOs.Infrastructure;

namespace Sudan_Train.Core.Features.Infrastructure.Trips.Queries.GetSegmentSeats
{
    public class GetSegmentSeatsQuery : IRequest<Response<SegmentSeatsDto>>
    {
        public int TripId { get; set; }
        public int BoardingStationId { get; set; }
        public int AlightingStationId { get; set; }
    }
}
