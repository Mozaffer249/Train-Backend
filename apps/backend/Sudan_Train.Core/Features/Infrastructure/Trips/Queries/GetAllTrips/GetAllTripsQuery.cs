using MediatR;
using Sudan_Train.Core.Bases;
using Sudan_Train.Data.DTOs.Infrastructure;

namespace Sudan_Train.Core.Features.Infrastructure.Trips.Queries.GetAllTrips
{
    public class GetAllTripsQuery : IRequest<Response<List<TripDto>>>
    {
        public DateTime? Date { get; set; }
        public int? RouteId { get; set; }
        public string? Status { get; set; }
        // When true, return only trips whose DepartureTime is in the future
        // (server clock). Used by the counter-sale flow so agents can't pick
        // a trip that has already departed.
        public bool? UpcomingOnly { get; set; }
    }
}

