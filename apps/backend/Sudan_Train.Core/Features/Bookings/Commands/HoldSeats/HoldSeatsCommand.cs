using MediatR;
using Sudan_Train.Core.Bases;
using Sudan_Train.Data.DTOs.Booking;

namespace Sudan_Train.Core.Features.Bookings.Commands.HoldSeats
{
    public class HoldSeatsCommand : IRequest<Response<SeatHoldResultDto>>
    {
        public int TripId { get; set; }
        public int BoardingStationId { get; set; }
        public int AlightingStationId { get; set; }
        public List<int> SeatIds { get; set; } = new();
        public Guid? HoldGroupId { get; set; }
    }
}
