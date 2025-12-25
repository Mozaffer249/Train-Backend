using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sudan_Train.Base;
using Sudan_Train.Core.Features.Authentication.Commands.TrustDevice;
using Sudan_Train.Core.Features.Authentication.Commands.RemoveTrustedDevice;
using Sudan_Train.Core.Features.Authentication.Queries.GetTrustedDevices;
using Sudan_Train.Core.Features.Authentication.Queries.GetSecurityEvents;
using Sudan_Train.Data.AppMetaData;

namespace Sudan_Train.Controllers.Authentication.Account
{
    /// <summary>
    /// Trusted device management and security monitoring
    /// </summary>
    public class DeviceController : AppControllerBase
    {
        /// <summary>
        /// Get all trusted devices for current user
        /// </summary>
        /// <param name="query">Get trusted devices query</param>
        /// <returns>List of trusted devices with device info and trust date</returns>
        [Authorize]
        [HttpGet(Router.AccountGetTrustedDevices)]
        public async Task<IActionResult> GetTrustedDevices([FromQuery] GetTrustedDevicesQuery query)
        {
            return NewResult(await Mediator.Send(query));
        }

        /// <summary>
        /// Trust current device to skip 2FA verification on future logins
        /// </summary>
        /// <param name="command">Device ID, name, and optional fingerprint</param>
        /// <returns>Success message</returns>
        [Authorize]
        [HttpPost(Router.AccountTrustDevice)]
        public async Task<IActionResult> TrustDevice([FromBody] TrustDeviceCommand command)
        {
            return NewResult(await Mediator.Send(command));
        }

        /// <summary>
        /// Remove a device from trusted devices list
        /// </summary>
        /// <param name="command">Device ID to remove</param>
        /// <returns>Success message</returns>
        [Authorize]
        [HttpDelete(Router.AccountRemoveTrustedDevice)]
        public async Task<IActionResult> RemoveTrustedDevice([FromBody] RemoveTrustedDeviceCommand command)
        {
            return NewResult(await Mediator.Send(command));
        }

        /// <summary>
        /// Get security events history (login alerts, password changes, etc.)
        /// </summary>
        /// <param name="query">Get security events query with pagination</param>
        /// <returns>Paginated list of security events</returns>
        [Authorize]
        [HttpGet(Router.AccountGetSecurityEvents)]
        public async Task<IActionResult> GetSecurityEvents([FromQuery] GetSecurityEventsQuery query)
        {
            return NewResult(await Mediator.Send(query));
        }
    }
}
