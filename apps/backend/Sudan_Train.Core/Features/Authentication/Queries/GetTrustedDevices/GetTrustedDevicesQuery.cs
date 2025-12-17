using MediatR;
using Sudan_Train.Core.Bases;
using Sudan_Train.Data.Entity.Identity;
using System.Collections.Generic;

namespace Sudan_Train.Core.Features.Authentication.Queries.GetTrustedDevices
{
    public class GetTrustedDevicesQuery : IRequest<Response<List<TrustedDevice>>>
    {
    }
}
