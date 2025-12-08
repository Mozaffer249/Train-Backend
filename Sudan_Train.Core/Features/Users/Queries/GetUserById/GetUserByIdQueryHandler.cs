using MediatR;
using Microsoft.AspNetCore.Identity;
using Sudan_Train.Core.Features.Users.Queries.GetUserList;
using Sudan_Train.Core.Bases;
using Sudan_Train.Data.Entity.Identity;

namespace Sudan_Train.Core.Features.Users.Queries.GetUserById
{
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Response<UserDto>>
    {
        private readonly UserManager<User> _userManager;

        public GetUserByIdQueryHandler(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Response<UserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.Id.ToString());

            if (user == null)
            {
                return new Response<UserDto>("User not found");
            }

            var userDto = new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName!,
                Email = user.Email!,
                PhoneNumber = user.PhoneNumber,
                IsActive = user.IsActive
            };

            return new Response<UserDto>(userDto, "User retrieved successfully");
        }
    }
}

