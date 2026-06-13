using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources.Shared;
using Sudan_Train.Data.DTOs.Booking;
using Sudan_Train.Data.Entity;
using Sudan_Train.Infrastructure.context;

namespace Sudan_Train.Core.Features.Refunds.Queries.GetAllRefunds
{
    public class GetAllRefundsQueryHandler
        : ResponseHandler, IRequestHandler<GetAllRefundsQuery, Response<List<RefundDto>>>
    {
        private readonly ApplicationDBContext _db;

        public GetAllRefundsQueryHandler(
            ApplicationDBContext db,
            IStringLocalizer<SharedResources> localizer) : base(localizer)
        {
            _db = db;
        }

        public async Task<Response<List<RefundDto>>> Handle(GetAllRefundsQuery request, CancellationToken cancellationToken)
        {
            var q = _db.Refunds
                .AsNoTracking()
                .Include(r => r.Booking).ThenInclude(b => b.User)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Status) &&
                Enum.TryParse<RefundStatus>(request.Status, true, out var statusEnum))
            {
                q = q.Where(r => r.Status == statusEnum);
            }

            var rows = await q
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new RefundDto
                {
                    Id = r.Id,
                    RefundNumber = r.RefundNumber,
                    BookingId = r.BookingId,
                    BookingReference = r.Booking.Reference,
                    UserId = r.Booking.UserId,
                    UserFullName = r.Booking.User != null
                        ? (r.Booking.User.FirstName + " " + r.Booking.User.LastName).Trim()
                        : null,
                    Amount = r.Amount,
                    Currency = r.Currency,
                    Status = r.Status.ToString(),
                    Method = r.Method.ToString(),
                    Reason = r.Reason,
                    ProcessedAt = r.ProcessedAt,
                    CreatedAt = r.CreatedAt,
                })
                .ToListAsync(cancellationToken);

            return Success<List<RefundDto>>("Refunds loaded", rows);
        }
    }
}
