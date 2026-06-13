using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Features.Users.Queries.GetUserList;
using Sudan_Train.Data.Entity.Identity;
using Sudan_Train.Infrastructure.context;

namespace Sudan_Train.Core.Features.Users.Queries.GetUserById
{
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Response<UserDto>>
    {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDBContext _db;

        public GetUserByIdQueryHandler(UserManager<User> userManager, ApplicationDBContext db)
        {
            _userManager = userManager;
            _db = db;
        }

        public async Task<Response<UserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.Id.ToString());

            if (user == null)
                return new Response<UserDto>("User not found");

            var roles = await _userManager.GetRolesAsync(user);
            var stationIds = await _db.StaffStations
                .Where(s => s.UserId == user.Id)
                .Select(s => s.StationId)
                .ToListAsync(cancellationToken);

            var userDto = new UserDto
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
            };

            return new Response<UserDto>(userDto, "User retrieved successfully");
        }
    }
}
