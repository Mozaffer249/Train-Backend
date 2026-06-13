using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources.Shared;
using Sudan_Train.Data.Entity.Identity;
using Sudan_Train.Infrastructure.context;

namespace Sudan_Train.Core.Features.Authentication.Queries.GetMe
{
    public class GetMeQueryHandler : ResponseHandler, IRequestHandler<GetMeQuery, Response<MeDto>>
    {
        private readonly UserManager<User> _userManager;
        private readonly IHttpContextAccessor _http;
        private readonly ApplicationDBContext _db;

        public GetMeQueryHandler(
            UserManager<User> userManager,
            IHttpContextAccessor http,
            ApplicationDBContext db,
            IStringLocalizer<SharedResources> localizer) : base(localizer)
        {
            _userManager = userManager;
            _http = http;
            _db = db;
        }

        public async Task<Response<MeDto>> Handle(GetMeQuery request, CancellationToken cancellationToken)
        {
            var userIdClaim = _http.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? _http.HttpContext?.User.FindFirst("uid")?.Value;
            if (!int.TryParse(userIdClaim, out var userId) || userId <= 0)
                return Unauthorized<MeDto>("Not authenticated.");

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return NotFound<MeDto>("User not found.");

            var roles = (await _userManager.GetRolesAsync(user)).ToList();
            var stationIds = await _db.StaffStations
                .AsNoTracking()
                .Where(ss => ss.UserId == userId)
                .Select(ss => ss.StationId)
                .ToListAsync(cancellationToken);

            return Success<MeDto>("OK", new MeDto
            {
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                FullName = $"{user.FirstName} {user.LastName}".Trim(),
                Roles = roles,
                AssignedStationIds = stationIds,
            });
        }
    }
}
