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

namespace Sudan_Train.Core.Features.Users.Commands.AssignStaffStations
{
    public class AssignStaffStationsCommandHandler : ResponseHandler, IRequestHandler<AssignStaffStationsCommand, Response<List<int>>>
    {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDBContext _db;
        private readonly IHttpContextAccessor _http;

        public AssignStaffStationsCommandHandler(
            UserManager<User> userManager,
            ApplicationDBContext db,
            IHttpContextAccessor http,
            IStringLocalizer<SharedResources> localizer) : base(localizer)
        {
            _userManager = userManager;
            _db = db;
            _http = http;
        }

        public async Task<Response<List<int>>> Handle(AssignStaffStationsCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.Id.ToString());
            if (user == null)
                return NotFound<List<int>>("User not found.");

            var requested = request.StationIds.Distinct().ToList();

            // Verify all requested stations exist.
            if (requested.Count > 0)
            {
                var existingStationIds = await _db.Stations
                    .Where(s => requested.Contains(s.Id))
                    .Select(s => s.Id)
                    .ToListAsync(cancellationToken);
                var missing = requested.Except(existingStationIds).ToList();
                if (missing.Count > 0)
                    return BadRequest<List<int>>($"Unknown station IDs: {string.Join(", ", missing)}");
            }

            // Audit field — who's making the assignment.
            var actorIdRaw = _http.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? _http.HttpContext?.User.FindFirst("uid")?.Value;
            int.TryParse(actorIdRaw, out var actorId);

            var current = await _db.StaffStations
                .Where(s => s.UserId == user.Id)
                .ToListAsync(cancellationToken);

            var currentIds = current.Select(s => s.StationId).ToHashSet();
            var requestedSet = requested.ToHashSet();

            // Remove stale assignments.
            var toRemove = current.Where(s => !requestedSet.Contains(s.StationId)).ToList();
            _db.StaffStations.RemoveRange(toRemove);

            // Add new ones.
            foreach (var sid in requested.Where(s => !currentIds.Contains(s)))
            {
                _db.StaffStations.Add(new StaffStation
                {
                    UserId = user.Id,
                    StationId = sid,
                    AssignedAt = DateTime.UtcNow,
                    AssignedBy = actorId > 0 ? actorId : null,
                });
            }

            await _db.SaveChangesAsync(cancellationToken);

            return Success("Station assignments updated", requested);
        }
    }
}
