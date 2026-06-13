using MediatR;
using Sudan_Train.Core.Bases;

namespace Sudan_Train.Core.Features.Refunds.Commands.RejectRefund
{
    public class RejectRefundCommand : IRequest<Response<string>>
    {
        public int Id { get; set; }
        public string? Reason { get; set; }
    }
}
