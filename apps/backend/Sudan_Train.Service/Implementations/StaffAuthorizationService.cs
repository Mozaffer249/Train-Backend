using Microsoft.EntityFrameworkCore;
using Sudan_Train.Data.AppMetaData;
using Sudan_Train.Infrastructure.context;
using Sudan_Train.Service.Abstracts;

namespace Sudan_Train.Service.Implementations
{
    public class StaffAuthorizationService : IStaffAuthorizationService
    {
        private readonly ApplicationDBContext _db;

        public StaffAuthorizationService(ApplicationDBContext db)
        {
            _db = db;
        }

        public async Task<bool> CanOperateTripAsync(int userId, IEnumerable<string> roles, int tripId)
        {
            // Admin bypass — they operate any station's trips.
            if (roles != null && (roles.Contains(Roles.Admin) || roles.Contains(Roles.SuperAdmin)))
                return true;

            var trip = await _db.Trip
                .AsNoTracking()
                .Where(t => t.Id == tripId)
                .Select(t => new
                {
                    t.Id,
                    t.Route.OriginStationId,
                    t.Route.DestinationStationId,
                    RouteStationIds = t.Route.RouteStations.Select(rs => rs.StationId).ToList(),
                })
                .FirstOrDefaultAsync();

            if (trip == null) return false;

            var tripStationIds = new HashSet<int>(trip.RouteStationIds) { trip.OriginStationId, trip.DestinationStationId };

            var hasMatch = await _db.StaffStations
                .AsNoTracking()
                .AnyAsync(ss => ss.UserId == userId && tripStationIds.Contains(ss.StationId));

            return hasMatch;
        }

        public async Task<List<int>> GetAssignedStationIdsAsync(int userId)
        {
            return await _db.StaffStations
                .AsNoTracking()
                .Where(ss => ss.UserId == userId)
                .Select(ss => ss.StationId)
                .ToListAsync();
        }

        public bool IsAdmin(IEnumerable<string> roles)
        {
            if (roles == null) return false;
            return roles.Contains(Roles.SuperAdmin) || roles.Contains(Roles.Admin);
        }
    }
}
