using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Helpers;
using Sudan_Train.Core.Resources.Shared;
using Sudan_Train.Data.Entity.Identity;
using Sudan_Train.Infrastructure.context;

namespace Sudan_Train.Core.Features.Users.Commands.SetUserActive
{
    public class SetUserActiveCommandHandler : ResponseHandler, IRequestHandler<SetUserActiveCommand, Response<string>>
    {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDBContext _db;
        private readonly IHttpContextAccessor _http;

        public SetUserActiveCommandHandler(
            UserManager<User> userManager,
            ApplicationDBContext db,
            IHttpContextAccessor http,
            IStringLocalizer<SharedResources> localizer) : base(localizer)
        {
            _userManager = userManager;
            _db = db;
            _http = http;
        }

        public async Task<Response<string>> Handle(SetUserActiveCommand request, CancellationToken cancellationToken)
        {
            var callerRoles = UserManagementAuthorization.GetCallerRoles(_http);

            var user = await _userManager.FindByIdAsync(request.Id.ToString());
            if (user == null)
                return NotFound<string>("User not found.");

            var targetRoles = await _userManager.GetRolesAsync(user);
            if (!UserManagementAuthorization.CanManageTarget(callerRoles, targetRoles))
                return BadRequest<string>(UserManagementAuthorization.PrivilegedUserError);

            if (!request.IsActive &&
                await UserManagementAuthorization.IsLastSuperAdminAsync(_db, user, _userManager))
                return BadRequest<string>(UserManagementAuthorization.LastSuperAdminError);

            if (user.IsActive == request.IsActive)
                return Success(request.IsActive ? "User already active" : "User already inactive", string.Empty);

            user.IsActive = request.IsActive;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return BadRequest<string>(string.Join("; ", result.Errors.Select(e => e.Description)));

            return Success(request.IsActive ? "User enabled" : "User disabled", string.Empty);
        }
    }
}
