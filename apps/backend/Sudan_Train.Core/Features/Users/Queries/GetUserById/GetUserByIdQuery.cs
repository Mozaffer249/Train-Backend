using MediatR;
using Sudan_Train.Core.Features.Users.Queries.GetUserList;
using Sudan_Train.Core.Bases;

namespace Sudan_Train.Core.Features.Users.Queries.GetUserById
{
    public class GetUserByIdQuery : IRequest<Response<UserDto>>
    {
        public int Id { get; set; }
    }
}

