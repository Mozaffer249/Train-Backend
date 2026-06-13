using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Sudan_Train.Core.Wrappers;
using Sudan_Train.Data.Entity.Identity;
using Sudan_Train.Infrastructure.context;

namespace Sudan_Train.Core.Features.Users.Queries.GetUserList
{
    public class GetUserListQueryHandler : IRequestHandler<GetUserListQuery, PaginatedResult<UserDto>>
    {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDBContext _db;

        public GetUserListQueryHandler(UserManager<User> userManager, ApplicationDBContext db)
        {
            _userManager = userManager;
            _db = db;
        }

        public async Task<PaginatedResult<UserDto>> Handle(GetUserListQuery request, CancellationToken cancellationToken)
        {
            // Apply search + IsActive filter at the SQL layer.
            var query = _userManager.Users.AsQueryable();

            if (!string.IsNullOrEmpty(request.Filter.Search))
            {
                var s = request.Filter.Search;
                query = query.Where(u =>
                    u.FirstName.Contains(s) ||
                    u.LastName.Contains(s) ||
                    u.UserName!.Contains(s) ||
                    u.Email!.Contains(s));
            }

            if (request.IsActive.HasValue)
                query = query.Where(u => u.IsActive == request.IsActive.Value);

            // Role filter requires joining via Identity tables — we fetch the
            // matching user IDs first and intersect.
            HashSet<int>? roleFilteredIds = null;
            if (!string.IsNullOrEmpty(request.Role))
            {
                var inRole = await _userManager.GetUsersInRoleAsync(request.Role);
                roleFilteredIds = inRole.Select(u => u.Id).ToHashSet();
                query = query.Where(u => roleFilteredIds.Contains(u.Id));
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var pageUsers = await query
                .OrderBy(u => u.Id)
                .Skip((request.Filter.PageNumber - 1) * request.Filter.PageSize)
                .Take(request.Filter.PageSize)
                .ToListAsync(cancellationToken);

            // Bulk-fetch StationIds for the visible page.
            var pageIds = pageUsers.Select(u => u.Id).ToList();
            var stationsByUser = await _db.StaffStations
                .Where(s => pageIds.Contains(s.UserId))
                .GroupBy(s => s.UserId)
                .ToDictionaryAsync(g => g.Key, g => g.Select(s => s.StationId).ToList(), cancellationToken);

            // Fetch each user's roles via UserManager (one round-trip per user;
            // for the page-size of 20 that's fine).
            var users = new List<UserDto>(pageUsers.Count);
            foreach (var u in pageUsers)
            {
                var roles = await _userManager.GetRolesAsync(u);
                users.Add(new UserDto
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    UserName = u.UserName!,
                    Email = u.Email!,
                    PhoneNumber = u.PhoneNumber,
                    IsActive = u.IsActive,
                    Roles = roles.ToList(),
                    StationIds = stationsByUser.TryGetValue(u.Id, out var sids) ? sids : new List<int>(),
                });
            }

            return PaginatedResult<UserDto>.Success(users, totalCount, request.Filter.PageNumber, request.Filter.PageSize);
        }
    }
}
