using MediatR;
using Sudan_Train.Core.Bases;
using Sudan_Train.Data.Entity.Identity;
using System.Collections.Generic;

namespace Sudan_Train.Core.Features.Authentication.Queries.GetSecurityEvents
{
    public class GetSecurityEventsQuery : IRequest<Response<List<SecurityEvent>>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
