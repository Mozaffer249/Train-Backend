using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Features.Users.Queries.GetUserList;
using Sudan_Train.Core.Resources.Shared;
using Sudan_Train.Data.Entity.Identity;
using Sudan_Train.Infrastructure.context;

namespace Sudan_Train.Core.Features.Users.Commands.UpdateUser
{
    public class UpdateUserCommandHandler : ResponseHandler, IRequestHandler<UpdateUserCommand, Response<UserDto>>
    {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDBContext _db;

        public UpdateUserCommandHandler(
            UserManager<User> userManager,
            ApplicationDBContext db,
            IStringLocalizer<SharedResources> localizer) : base(localizer)
        {
            _userManager = userManager;
            _db = db;
        }

        public async Task<Response<UserDto>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.Id.ToString());
            if (user == null)
                return NotFound<UserDto>("User not found.");

            if (request.FirstName != null) user.FirstName = request.FirstName;
            if (request.LastName != null) user.LastName = request.LastName;
            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                user.Email = request.Email;
                user.NormalizedEmail = _userManager.NormalizeEmail(request.Email);
            }
            if (request.PhoneNumber != null) user.PhoneNumber = request.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return BadRequest<UserDto>(string.Join("; ", result.Errors.Select(e => e.Description)));

            var roles = await _userManager.GetRolesAsync(user);
            var stationIds = await _db.StaffStations
                .Where(s => s.UserId == user.Id)
                .Select(s => s.StationId)
                .ToListAsync(cancellationToken);

            return Success("User updated", new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName!,
                Email = user.Email!,
                PhoneNumber = user.PhoneNumber,
                IsActive = user.IsActive,
                Roles = roles.ToList(),
                StationIds = stationIds,
            });
        }
    }
}
