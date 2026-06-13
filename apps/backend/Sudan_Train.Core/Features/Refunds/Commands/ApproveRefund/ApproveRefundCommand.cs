using MediatR;
using Sudan_Train.Core.Bases;

namespace Sudan_Train.Core.Features.Refunds.Commands.ApproveRefund
{
    public class ApproveRefundCommand : IRequest<Response<string>>
    {
        public int Id { get; set; }
        public string? Note { get; set; }
    }
}
