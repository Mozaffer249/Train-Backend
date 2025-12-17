using MediatR;
using Sudan_Train.Core.Bases;

namespace Sudan_Train.Core.Features.Authentication.Commands.TrustDevice
{
    public class TrustDeviceCommand : IRequest<Response<string>>
    {
        public string DeviceId { get; set; } = default!;
        public string DeviceName { get; set; } = default!;
        public string? DeviceFingerprint { get; set; }
    }
}
