using MediatR;
using Sudan_Train.Core.Bases;
using Sudan_Train.Data.DTOs.Booking;

namespace Sudan_Train.Core.Features.Boarding.Queries.GetTripManifest
{
    public class GetTripManifestQuery : IRequest<Response<TripManifestDto>>
    {
        public int TripId { get; set; }
        public int? BoardingStationId { get; set; }
    }
}
