using MediatR;
using Sudan_Train.Core.Bases;

namespace Sudan_Train.Core.Features.Authentication.Commands.RemoveTrustedDevice
{
    public class RemoveTrustedDeviceCommand : IRequest<Response<string>>
    {
        public int DeviceId { get; set; }
    }
}
