using MediatR;
using Sudan_Train.Core.Filters;
using Sudan_Train.Core.Wrappers;

namespace Sudan_Train.Core.Features.Users.Queries.GetUserList
{
    public class GetUserListQuery : IRequest<PaginatedResult<UserDto>>
    {
        public PaginatedListFilter Filter { get; set; } = default!;
        // Optional filters set by the admin UsersPage. Null = no filter.
        public string? Role { get; set; }
        public bool? IsActive { get; set; }
    }

    public class UserDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string UserName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string? PhoneNumber { get; set; }
        public bool IsActive { get; set; }

        // Identity roles assigned to this user.
        public List<string> Roles { get; set; } = new();

        // Stations the user is assigned to via StaffStation join. Empty for
        // non-staff users.
        public List<int> StationIds { get; set; } = new();
    }
}

