using MediatR;
using Microsoft.AspNetCore.Identity;
using Sudan_Train.Core.Wrappers;
using Sudan_Train.Data.Entity.Identity;

namespace Sudan_Train.Core.Features.Users.Queries.GetUserList
{
    public class GetUserListQueryHandler : IRequestHandler<GetUserListQuery, PaginatedResult<UserDto>>
    {
        private readonly UserManager<User> _userManager;

        public GetUserListQueryHandler(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<PaginatedResult<UserDto>> Handle(GetUserListQuery request, CancellationToken cancellationToken)
        {
            var allUsers = _userManager.Users;

            // Apply search filter if provided
            if (!string.IsNullOrEmpty(request.Filter.Search))
            {
                allUsers = allUsers.Where(u =>
                    u.FirstName.Contains(request.Filter.Search) ||
                    u.LastName.Contains(request.Filter.Search) ||
                    u.UserName!.Contains(request.Filter.Search) ||
                    u.Email!.Contains(request.Filter.Search));
            }

            // Get total count
            var totalCount = allUsers.Count();

            // Apply pagination and convert to DTO
            var users = allUsers
                .Skip((request.Filter.PageNumber - 1) * request.Filter.PageSize)
                .Take(request.Filter.PageSize)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    UserName = u.UserName!,
                    Email = u.Email!,
                    PhoneNumber = u.PhoneNumber,
                    IsActive = u.IsActive
                })
                .ToList();

            return PaginatedResult<UserDto>.Success(users, totalCount, request.Filter.PageNumber, request.Filter.PageSize);
        }
    }
}

