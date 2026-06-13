using MediatR;
using Sudan_Train.Core.Bases;
using Sudan_Train.Data.DTOs.Booking;

namespace Sudan_Train.Core.Features.Refunds.Queries.GetAllRefunds
{
    public class GetAllRefundsQuery : IRequest<Response<List<RefundDto>>>
    {
        // Optional status filter — Pending / Approved / Rejected / Completed.
        public string? Status { get; set; }
    }
}
