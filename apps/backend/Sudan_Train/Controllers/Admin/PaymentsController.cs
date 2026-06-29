using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sudan_Train.Core.Features.Payments.Queries.GetPaymentsReport;
using Sudan_Train.Data.AppMetaData;

namespace Sudan_Train.Controllers.Admin
{
    [ApiController]
    [Route(Router.Rule + "Payments")]
    public class PaymentsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PaymentsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>Payments report with filters and summary totals. Admin-only.</summary>
        [HttpGet]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> Report(
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null,
            [FromQuery] string? method = null,
            [FromQuery] string? status = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20)
        {
            var response = await _mediator.Send(new GetPaymentsReportQuery
            {
                FromDate = fromDate,
                ToDate = toDate,
                Method = method,
                Status = status,
                PageNumber = pageNumber,
                PageSize = pageSize,
            });
            return Ok(response);
        }
    }
}
