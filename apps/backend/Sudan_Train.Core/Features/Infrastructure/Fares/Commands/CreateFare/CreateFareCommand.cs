using MediatR;
using Sudan_Train.Core.Bases;
using Sudan_Train.Data.DTOs.Infrastructure;
using Sudan_Train.Data.Entity;

namespace Sudan_Train.Core.Features.Infrastructure.Fares.Commands.CreateFare
{
    public class CreateFareCommand : IRequest<Response<FareDto>>
    {
        public int? RouteId { get; set; }
        public int? OriginStationId { get; set; }
        public int? DestinationStationId { get; set; }
        public int? TripId { get; set; }
        public CoachClass CoachClass { get; set; }
        public decimal BasePrice { get; set; }
        public decimal? DiscountPercent { get; set; }
    }
}
