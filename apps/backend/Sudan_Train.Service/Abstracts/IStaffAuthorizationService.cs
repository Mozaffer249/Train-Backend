namespace Sudan_Train.Service.Abstracts
{
    // Resolves whether the current user is allowed to operate (board, mark
    // no-show, depart/arrive) on a given trip. Admin/SuperAdmin always pass;
    // Staff (boarding/counter) must have at least one StaffStation matching a
    // station that this trip touches (origin, destination, or an intermediate
    // stop on the route).
    public interface IStaffAuthorizationService
    {
        Task<bool> CanOperateTripAsync(int userId, IEnumerable<string> roles, int tripId);
        Task<List<int>> GetAssignedStationIdsAsync(int userId);

        // True when the role set includes SuperAdmin or Admin. Handlers
        // branch on this to skip station-scope checks.
        bool IsAdmin(IEnumerable<string> roles);
    }
}
