using MediatR;
using Sudan_Train.Core.Bases;

namespace Sudan_Train.Core.Features.Boarding.Commands.ScanTicket
{
    // QrPayload is whatever the scanner decoded — either the full QR JSON
    // payload created at issue time OR (for manual fallback) just a ticket
    // number string. The handler tries both.
    public class ScanTicketCommand : IRequest<Response<ScanTicketResultDto>>
    {
        public string QrPayload { get; set; } = default!;
    }

    public class ScanTicketResultDto
    {
        public int TicketId { get; set; }
        public string? TicketNumber { get; set; }
        public string Status { get; set; } = "Boarded";
        public string? PassengerName { get; set; }
        public string? SeatNumber { get; set; }
        public int TripId { get; set; }
    }
}
