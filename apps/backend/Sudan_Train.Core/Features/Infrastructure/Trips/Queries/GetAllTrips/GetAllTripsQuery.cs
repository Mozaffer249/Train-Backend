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
    }
}

