using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources.Shared;
using Sudan_Train.Data.Entity;
using Sudan_Train.Infrastructure.context;

namespace Sudan_Train.Core.Features.Refunds.Commands.RejectRefund
{
    public class RejectRefundCommandHandler
        : ResponseHandler, IRequestHandler<RejectRefundCommand, Response<string>>
    {
        private readonly ApplicationDBContext _db;

        public RejectRefundCommandHandler(
            ApplicationDBContext db,
            IStringLocalizer<SharedResources> localizer) : base(localizer)
        {
            _db = db;
        }

        public async Task<Response<string>> Handle(RejectRefundCommand request, CancellationToken cancellationToken)
        {
            var refund = await _db.Refunds.FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);
            if (refund == null)
                return NotFound<string>("Refund not found.");

            if (refund.Status == RefundStatus.Completed)
                return BadRequest<string>("Refund already completed.");
            if (refund.Status == RefundStatus.Rejected)
                return Success<string>("Refund already rejected.");

            refund.Status = RefundStatus.Rejected;
            refund.ProcessedAt = DateTime.UtcNow;
            if (!string.IsNullOrWhiteSpace(request.Reason))
                refund.ProcessorResponse = request.Reason;

            await _db.SaveChangesAsync(cancellationToken);
            return Success<string>("Refund rejected.");
        }
    }
}
