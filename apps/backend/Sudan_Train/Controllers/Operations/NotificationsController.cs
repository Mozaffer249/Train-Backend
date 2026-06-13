using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sudan_Train.Core.Features.Notifications.Commands.MarkNotificationRead;
using Sudan_Train.Core.Features.Notifications.Queries.GetMyNotifications;
using Sudan_Train.Data.AppMetaData;

namespace Sudan_Train.Controllers.Operations
{
    [ApiController]
    [Route(Router.Rule + "Notifications")]
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public NotificationsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>Current user's notifications (most recent 50).</summary>
        [HttpGet("Mine")]
        public async Task<IActionResult> Mine([FromQuery] bool? unreadOnly = null)
        {
            var response = await _mediator.Send(new GetMyNotificationsQuery { UnreadOnly = unreadOnly });
            return Ok(response);
        }

        /// <summary>Mark a single notification as read.</summary>
        [HttpPost("{id:int}/Read")]
        public async Task<IActionResult> MarkRead(int id)
        {
            var response = await _mediator.Send(new MarkNotificationReadCommand { Id = id });
            return Ok(response);
        }
    }
}
