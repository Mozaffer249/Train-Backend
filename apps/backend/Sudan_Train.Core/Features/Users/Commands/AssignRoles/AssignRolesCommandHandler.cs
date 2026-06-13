using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources.Shared;
using Sudan_Train.Data.Entity.Identity;

namespace Sudan_Train.Core.Features.Users.Commands.AssignRoles
{
    public class AssignRolesCommandHandler : ResponseHandler, IRequestHandler<AssignRolesCommand, Response<List<string>>>
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;

        public AssignRolesCommandHandler(
            UserManager<User> userManager,
            RoleManager<Role> roleManager,
            IStringLocalizer<SharedResources> localizer) : base(localizer)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<Response<List<string>>> Handle(AssignRolesCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.Id.ToString());
            if (user == null)
                return NotFound<List<string>>("User not found.");

            var requested = request.Roles.Distinct().ToList();

            // Reject unknown roles up front.
            foreach (var r in requested)
            {
                if (!await _roleManager.RoleExistsAsync(r))
                    return BadRequest<List<string>>($"Role '{r}' does not exist.");
            }

            var current = (await _userManager.GetRolesAsync(user)).ToList();
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
