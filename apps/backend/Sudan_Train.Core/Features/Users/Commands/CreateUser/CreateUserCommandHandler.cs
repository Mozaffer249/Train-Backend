using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Features.Users.Queries.GetUserList;
using Sudan_Train.Core.Resources.Shared;
using Sudan_Train.Data.Entity.Identity;
using Sudan_Train.Infrastructure.context;

namespace Sudan_Train.Core.Features.Users.Commands.CreateUser
{
    public class CreateUserCommandHandler : ResponseHandler, IRequestHandler<CreateUserCommand, Response<UserDto>>
    {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDBContext _db;

        public CreateUserCommandHandler(
            UserManager<User> userManager,
            ApplicationDBContext db,
            IStringLocalizer<SharedResources> localizer) : base(localizer)
        {
            _userManager = userManager;
            _db = db;
        }

        public async Task<Response<UserDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            // Uniqueness checks up front so the error is clear.
            if (await _userManager.FindByEmailAsync(request.Email) != null)
                return BadRequest<UserDto>("Email is already in use.");
            if (await _userManager.FindByNameAsync(request.UserName) != null)
                return BadRequest<UserDto>("Username is already in use.");

            var user = new User
            {
                UserName = request.UserName,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber,
                EmailConfirmed = true,        // admin-created accounts are pre-confirmed
                IsActive = true,
            };

            var createResult = await _userManager.CreateAsync(user, request.Password);
            if (!createResult.Succeeded)
                return BadRequest<UserDto>(string.Join("; ", createResult.Errors.Select(e => e.Description)));

            // Assign initial roles.
            if (request.Roles.Count > 0)
            {
                var rolesResult = await _userManager.AddToRolesAsync(user, request.Roles);
                if (!rolesResult.Succeeded)
                    return BadRequest<UserDto>(string.Join("; ", rolesResult.Errors.Select(e => e.Description)));
            }

            // Assign station scope if any. Only meaningful for staff roles
            // but we don't enforce — admin may grant in advance.
            if (request.StationIds.Count > 0)
            {
                foreach (var sid in request.StationIds.Distinct())
                {
                    _db.StaffStations.Add(new StaffStation
                    {
                        UserId = user.Id,
                        StationId = sid,
                        AssignedAt = DateTime.UtcNow,
                    });
                }
                await _db.SaveChangesAsync(cancellationToken);
            }

            var roles = await _userManager.GetRolesAsync(user);
            return Created("User created", new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName!,
                Email = user.Email!,
                PhoneNumber = user.PhoneNumber,
                IsActive = user.IsActive,
                Roles = roles.ToList(),
                StationIds = request.StationIds.Distinct().ToList(),
            });
        }
    }
}
