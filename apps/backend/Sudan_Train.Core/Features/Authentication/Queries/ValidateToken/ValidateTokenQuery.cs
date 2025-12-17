using MediatR;
using Sudan_Train.Core.Bases;

namespace Sudan_Train.Core.Features.Authentication.Queries.ValidateToken
{
    public class ValidateTokenQuery : IRequest<Response<string>>
    {
        public string AccessToken { get; set; } = default!;
    }
}
