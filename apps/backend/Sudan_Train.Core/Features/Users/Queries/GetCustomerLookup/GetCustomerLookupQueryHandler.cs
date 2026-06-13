using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources.Shared;
using Sudan_Train.Data.Entity.Identity;
using Sudan_Train.Infrastructure.context;

namespace Sudan_Train.Core.Features.Users.Queries.GetCustomerLookup
{
    public class GetCustomerLookupQueryHandler : ResponseHandler, IRequestHandler<GetCustomerLookupQuery, Response<List<CustomerSummaryDto>>>
    {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDBContext _db;

        public GetCustomerLookupQueryHandler(
            UserManager<User> userManager,
            ApplicationDBContext db,
            IStringLocalizer<SharedResources> localizer) : base(localizer)
        {
            _userManager = userManager;
            _db = db;
        }

        public async Task<Response<List<CustomerSummaryDto>>> Handle(GetCustomerLookupQuery request, CancellationToken cancellationToken)
        {
            var q = (request.Query ?? string.Empty).Trim();
            if (q.Length < 2)
                return Success<List<CustomerSummaryDto>>(null, new());

            // Match users by phone / email / username / first+last name. Active
            // accounts only so disabled staff/customers don't show up.
            var users = await _userManager.Users
                .Where(u => u.IsActive)
                .Where(u =>
                    (u.PhoneNumber != null && u.PhoneNumber.Contains(q)) ||
                    (u.Email != null && u.Email.Contains(q)) ||
                    (u.UserName != null && u.UserName.Contains(q)) ||
                    u.FirstName.Contains(q) ||
                    u.LastName.Contains(q))
                .OrderBy(u => u.Id)
                .Take(10)
                .Select(u => new
                {
                    u.Id, u.FirstName, u.LastName, u.Email, u.PhoneNumber, u.UserName,
                })
                .ToListAsync(cancellationToken);

            // Also match by passenger.IdNumber from prior bookings. Each match
            // resolves back to the user via Passenger.UserId (if set).
            // EncryptColumn on Passenger.IdNumber means we can't .Contains in SQL —
            // skip this branch for now and just use the user-table match. (The
            // counter form can still find walk-in customers since they don't
            // have an account anyway.)

            var userIds = users.Select(u => u.Id).ToList();
            // Look up most recent passenger.IdNumber per user (for display).
            var idByUser = await _db.Passengers
                .Where(p => p.UserId != null && userIds.Contains(p.UserId.Value))
                .GroupBy(p => p.UserId!.Value)
                .ToDictionaryAsync(g => g.Key, g => g.First().IdNumber, cancellationToken);

            var result = users.Select(u => new CustomerSummaryDto
            {
                UserId = u.Id,
                FullName = $"{u.FirstName} {u.LastName}".Trim(),
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                UserName = u.UserName,
                IdNumber = idByUser.TryGetValue(u.Id, out var id) ? id : null,
            }).ToList();

            return Success<List<CustomerSummaryDto>>(null, result);
        }
    }
}
