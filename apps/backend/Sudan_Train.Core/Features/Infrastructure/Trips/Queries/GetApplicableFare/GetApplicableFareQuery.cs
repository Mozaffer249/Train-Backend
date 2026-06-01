using MediatR;
using Sudan_Train.Core.Bases;
using Sudan_Train.Data.DTOs.Infrastructure;
using Sudan_Train.Data.Entity;

namespace Sudan_Train.Core.Features.Infrastructure.Trips.Queries.GetApplicableFare
{
    public class GetApplicableFareQuery : IRequest<Response<FareDto>>
    {
        public int TripId { get; set; }
        public int BoardingStationId { get; set; }
        public int AlightingStationId { get; set; }
        // Optional. When omitted the resolver returns the cheapest fare across
        // any class for this trip+segment — used by the search list where no
        // class has been chosen. The booking flow always passes a concrete class.
        public CoachClass? CoachClass { get; set; }
    }
}
