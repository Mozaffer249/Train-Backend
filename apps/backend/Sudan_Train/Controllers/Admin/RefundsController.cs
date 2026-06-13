using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sudan_Train.Core.Features.Refunds.Commands.ApproveRefund;
using Sudan_Train.Core.Features.Refunds.Commands.RejectRefund;
using Sudan_Train.Core.Features.Refunds.Queries.GetAllRefunds;
using Sudan_Train.Data.AppMetaData;

namespace Sudan_Train.Controllers.Admin
{
    [ApiController]
    [Route(Router.Rule + "Refunds")]
    public class RefundsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RefundsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>List refunds (optionally filter by status). Admin-only.</summary>
        [HttpGet]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> List([FromQuery] string? status = null)
        {
            var response = await _mediator.Send(new GetAllRefundsQuery { Status = status });
            return Ok(response);
        }

        /// <summary>Approve a refund (mock — flips status to Completed).</summary>
        [HttpPost("{id:int}/Approve")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> Approve(int id, [FromBody] ApproveRefundCommand? body = null)
        {
            var command = body ?? new ApproveRefundCommand();
            command.Id = id;
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        /// <summary>Reject a refund.</summary>
        [HttpPost("{id:int}/Reject")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> Reject(int id, [FromBody] RejectRefundCommand? body = null)
        {
            var command = body ?? new RejectRefundCommand();
            command.Id = id;
            var response = await _mediator.Send(command);
            return Ok(response);
        }
    }
}
