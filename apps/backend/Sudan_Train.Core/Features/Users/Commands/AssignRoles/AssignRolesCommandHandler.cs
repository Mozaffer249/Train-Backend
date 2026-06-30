using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Helpers;
using Sudan_Train.Core.Resources.Shared;
using Sudan_Train.Data.Entity.Identity;

using Sudan_Train.Infrastructure.context;

namespace Sudan_Train.Core.Features.Users.Commands.AssignRoles
{
    public class AssignRolesCommandHandler : ResponseHandler, IRequestHandler<AssignRolesCommand, Response<List<string>>>
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IHttpContextAccessor _http;
        private readonly ApplicationDBContext _db;

        public AssignRolesCommandHandler(
            UserManager<User> userManager,
            RoleManager<Role> roleManager,
            IHttpContextAccessor http,
            ApplicationDBContext db,
            IStringLocalizer<SharedResources> localizer) : base(localizer)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _http = http;
            _db = db;
        }

        public async Task<Response<List<string>>> Handle(AssignRolesCommand request, CancellationToken cancellationToken)
        {
            var callerRoles = UserManagementAuthorization.GetCallerRoles(_http);

            var user = await _userManager.FindByIdAsync(request.Id.ToString());
            if (user == null)
                return NotFound<List<string>>("User not found.");

            var current = (await _userManager.GetRolesAsync(user)).ToList();
            var requested = request.Roles.Distinct().ToList();

            if (!UserManagementAuthorization.CanManageTarget(callerRoles, current))
                return BadRequest<List<string>>(UserManagementAuthorization.PrivilegedUserError);

            if (!UserManagementAuthorization.CanAssignRequestedRoles(callerRoles, requested))
                return BadRequest<List<string>>(UserManagementAuthorization.PrivilegedRoleAssignError);

            if (await UserManagementAuthorization.WouldRemoveLastSuperAdminAsync(_userManager, _db, user, requested))
                return BadRequest<List<string>>(UserManagementAuthorization.LastSuperAdminError);

            foreach (var r in requested)
            {
                if (!await _roleManager.RoleExistsAsync(r))
                    return BadRequest<List<string>>($"Role '{r}' does not exist.");
            }

            var toAdd = requested.Except(current).ToList();
            var toRemove = current.Except(requested).ToList();

            if (toRemove.Count > 0)
            {
                var removeResult = await _userManager.RemoveFromRolesAsync(user, toRemove);
                if (!removeResult.Succeeded)
                    return BadRequest<List<string>>(string.Join("; ", removeResult.Errors.Select(e => e.Description)));
            }

            if (toAdd.Count > 0)
            {
                var addResult = await _userManager.AddToRolesAsync(user, toAdd);
                if (!addResult.Succeeded)
                    return BadRequest<List<string>>(string.Join("; ", addResult.Errors.Select(e => e.Description)));
            }

            var finalRoles = (await _userManager.GetRolesAsync(user)).ToList();
            return Success("Roles updated", finalRoles);
        }
    }
}
