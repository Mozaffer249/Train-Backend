using MediatR;
using Sudan_Train.Core.Bases;
using Sudan_Train.Data.DTOs.Infrastructure;

namespace Sudan_Train.Core.Features.Infrastructure.Fares.Commands.UpdateFare
{
    // PATCH-style update — every field nullable. Scope columns
    // (RouteId/OriginStationId/DestinationStationId/TripId/CoachClass) are NOT
    // editable here; admins retire + recreate when the scope is wrong.
    public class UpdateFareCommand : IRequest<Response<FareDto>>
    {
        public int Id { get; set; }
        public decimal? BasePrice { get; set; }
        public decimal? DiscountPercent { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
    }
}
