using MediatR;
using Sudan_Train.Core.Filters;
using Sudan_Train.Core.Wrappers;

namespace Sudan_Train.Core.Features.Users.Queries.GetUserList
{
    public class GetUserListQuery : IRequest<PaginatedResult<UserDto>>
    {
        public PaginatedListFilter Filter { get; set; } = default!;
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
    }
}

