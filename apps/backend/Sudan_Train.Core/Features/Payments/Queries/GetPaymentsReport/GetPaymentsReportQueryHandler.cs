using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources.Shared;
using Sudan_Train.Data.DTOs.Payment;
using Sudan_Train.Data.Entity;
using Sudan_Train.Infrastructure.context;

namespace Sudan_Train.Core.Features.Payments.Queries.GetPaymentsReport
{
    public class GetPaymentsReportQueryHandler
        : ResponseHandler, IRequestHandler<GetPaymentsReportQuery, Response<PaymentsReportDto>>
    {
        private readonly ApplicationDBContext _db;

        public GetPaymentsReportQueryHandler(
            ApplicationDBContext db,
            IStringLocalizer<SharedResources> localizer) : base(localizer)
        {
            _db = db;
        }

        public async Task<Response<PaymentsReportDto>> Handle(GetPaymentsReportQuery request, CancellationToken cancellationToken)
        {
            var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
            var pageSize = request.PageSize < 1 ? 20 : Math.Min(request.PageSize, 100);

            var q = _db.Payments
                .AsNoTracking()
                .Include(p => p.Booking).ThenInclude(b => b.User)
                .AsQueryable();

            if (request.FromDate.HasValue)
            {
                var from = request.FromDate.Value.Date;
                q = q.Where(p => p.CreatedAt >= from);
            }

            if (request.ToDate.HasValue)
            {
                var toExclusive = request.ToDate.Value.Date.AddDays(1);
                q = q.Where(p => p.CreatedAt < toExclusive);
            }

            if (!string.IsNullOrWhiteSpace(request.Method) &&
                Enum.TryParse<PaymentMethod>(request.Method, true, out var methodEnum))
            {
                q = q.Where(p => p.Method == methodEnum);
            }

            if (!string.IsNullOrWhiteSpace(request.Status) &&
                Enum.TryParse<PaymentStatus>(request.Status, true, out var statusEnum))
            {
                q = q.Where(p => p.Status == statusEnum);
            }

            var totalCount = await q.CountAsync(cancellationToken);

            var summaryRows = await q
                .Select(p => new { p.Status, p.Method, p.Amount })
                .ToListAsync(cancellationToken);

            var summary = new PaymentsSummaryDto
            {
                Count = summaryRows.Count,
                TotalCollected = summaryRows
                    .Where(r => r.Status == PaymentStatus.Completed)
                    .Sum(r => r.Amount),
                ByStatus = summaryRows
                    .GroupBy(r => r.Status)
                    .Select(g => new PaymentStatusCountDto
                    {
                        Status = g.Key.ToString(),
                        Count = g.Count(),
                        Amount = g.Sum(x => x.Amount),
                    })
                    .OrderBy(x => x.Status)
                    .ToList(),
                ByMethod = summaryRows
                    .GroupBy(r => r.Method)
                    .Select(g => new PaymentMethodCountDto
                    {
                        Method = g.Key.ToString(),
                        Count = g.Count(),
                        Amount = g.Sum(x => x.Amount),
                    })
                    .OrderBy(x => x.Method)
                    .ToList(),
            };

            var items = await q
                .OrderByDescending(p => p.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new PaymentDto
                {
                    Id = p.Id,
                    BookingId = p.BookingId,
                    BookingRef = p.Booking.Reference,
                    CustomerName = p.Booking.User != null
                        ? (p.Booking.User.FirstName + " " + p.Booking.User.LastName).Trim()
                        : null,
                    Method = p.Method.ToString(),
                    Status = p.Status.ToString(),
                    Amount = p.Amount,
                    Currency = p.Currency,
                    CardBrand = p.CardBrand,
                    CardLast4 = p.CardLast4,
                    Reference = p.Reference,
                    CreatedAt = p.CreatedAt,
                })
                .ToListAsync(cancellationToken);

            var report = new PaymentsReportDto
            {
                Items = items,
                Summary = summary,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
            };

            return Success("Payments report loaded", report);
        }
    }
}
