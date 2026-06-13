using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources.Shared;
using Sudan_Train.Data.Entity;
using Sudan_Train.Infrastructure.context;

namespace Sudan_Train.Core.Features.Refunds.Commands.ApproveRefund
{
    public class ApproveRefundCommandHandler
        : ResponseHandler, IRequestHandler<ApproveRefundCommand, Response<string>>
    {
        private readonly ApplicationDBContext _db;

        public ApproveRefundCommandHandler(
            ApplicationDBContext db,
            IStringLocalizer<SharedResources> localizer) : base(localizer)
        {
            _db = db;
        }

        public async Task<Response<string>> Handle(ApproveRefundCommand request, CancellationToken cancellationToken)
        {
            var refund = await _db.Refunds.FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);
            if (refund == null)
                return NotFound<string>("Refund not found.");

            if (refund.Status == RefundStatus.Completed)
                return Success<string>("Refund already completed.");
            if (refund.Status == RefundStatus.Rejected)
                return BadRequest<string>("Refund was rejected.");

            // Mock approval — no real payment-gateway call. Flip straight to
            // Completed so the customer sees the resolution.
            refund.Status = RefundStatus.Completed;
            refund.ProcessedAt = DateTime.UtcNow;
            if (!string.IsNullOrWhiteSpace(request.Note))
                refund.ProcessorResponse = request.Note;

            await _db.SaveChangesAsync(cancellationToken);
            return Success<string>("Refund approved.");
        }
    }
}
