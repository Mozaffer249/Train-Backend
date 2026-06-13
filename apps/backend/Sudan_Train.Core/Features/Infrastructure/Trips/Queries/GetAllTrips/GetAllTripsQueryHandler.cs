using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources.Shared;
using Sudan_Train.Data.DTOs.Infrastructure;
using Sudan_Train.Service.Abstracts;

namespace Sudan_Train.Core.Features.Infrastructure.Trips.Queries.GetAllTrips
{
    public class GetAllTripsQueryHandler : ResponseHandler, IRequestHandler<GetAllTripsQuery, Response<List<TripDto>>>
    {
        private readonly ITripService _tripService;
        private readonly IHttpContextAccessor _http;
        private readonly IStaffAuthorizationService _staffAuth;

        public GetAllTripsQueryHandler(
            ITripService tripService,
            IHttpContextAccessor http,
            IStaffAuthorizationService staffAuth,
            IStringLocalizer<SharedResources> localizer) : base(localizer)
        {
            _tripService = tripService;
            _http = http;
            _staffAuth = staffAuth;
        }

        public async Task<Response<List<TripDto>>> Handle(GetAllTripsQuery request, CancellationToken cancellationToken)
        {
            // Station-scope filter: applied only when the JWT carries a
            // station-staff sub-role (StaffCounter or StaffBoarding) AND the
            // caller is NOT an admin. Anonymous + customer + admin all see
            // the unfiltered list.
            List<int>? assignedStationIds = null;
            var http = _http.HttpContext;
            if (http?.User?.Identity?.IsAuthenticated == true)
            {
                var roles = http.User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
                if (!_staffAuth.IsAdmin(roles))
                {
                    var userIdClaim = http.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                        ?? http.User.FindFirst("uid")?.Value;
                    if (int.TryParse(userIdClaim, out var userId) && userId > 0)
                    {
                        var assigned = await _staffAuth.GetAssignedStationIdsAsync(userId);
                        // Only narrow the list when this is a station-staff role.
                        // Customers (no StaffStation rows) see the unfiltered list.
                        if (assigned.Count > 0)
                            assignedStationIds = assigned;
                    }
                }
            }

            var trips = await _tripService.GetAllTripsAsync(
                request.Date,
                request.RouteId,
                request.Status,
                assignedStationIds,
                request.UpcomingOnly ?? false);
            return Success(null, trips);
        }
    }
}
