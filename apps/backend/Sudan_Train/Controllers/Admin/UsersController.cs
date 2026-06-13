using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sudan_Train.Core.Features.Users.Commands.AssignRoles;
using Sudan_Train.Core.Features.Users.Commands.AssignStaffStations;
using Sudan_Train.Core.Features.Users.Commands.CreateUser;
using Sudan_Train.Core.Features.Users.Commands.SetUserActive;
using Sudan_Train.Core.Features.Users.Commands.UpdateUser;
using Sudan_Train.Core.Features.Users.Queries.GetCustomerLookup;
using Sudan_Train.Core.Features.Users.Queries.GetUserById;
using Sudan_Train.Core.Features.Users.Queries.GetUserList;
using Sudan_Train.Core.Filters;
using Sudan_Train.Data.AppMetaData;

namespace Sudan_Train.Controllers.Admin
{
    [ApiController]
    [Route(Router.Admin)]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // ----- Reads (AnyStaff: Admin/Staff can list for filtering manifests). -----

        [HttpGet("Users")]
        [Authorize(Roles = Roles.AnyStaff)]
        public async Task<IActionResult> List(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? search = null,
            [FromQuery] string? role = null,
            [FromQuery] bool? isActive = null)
        {
            var query = new GetUserListQuery
            {
                Filter = new PaginatedListFilter { PageNumber = pageNumber, PageSize = pageSize, Search = search },
                Role = role,
                IsActive = isActive,
            };
            var response = await _mediator.Send(query);
            return Ok(response);
        }

        [HttpGet("Users/{id:int}")]
        [Authorize(Roles = Roles.AnyStaff)]
        public async Task<IActionResult> GetById(int id)
        {
            var response = await _mediator.Send(new GetUserByIdQuery { Id = id });
            return Ok(response);
        }

        // ----- Counter-flow customer lookup (Staff counter + Admin). -----

        [HttpGet("Users/Lookup")]
        [Authorize(Roles = Roles.CounterRoles)]
        public async Task<IActionResult> Lookup([FromQuery] string query)
        {
            var response = await _mediator.Send(new GetCustomerLookupQuery { Query = query });
            return Ok(response);
        }

        // ----- Writes (SuperAdmin/Admin only). -----

        [HttpPost("Users")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> Create([FromBody] CreateUserCommand command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPut("Users/{id:int}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateUserCommand command)
        {
            command.Id = id;
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPut("Users/{id:int}/Active")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> SetActive(int id, [FromBody] SetUserActiveCommand command)
        {
            command.Id = id;
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPut("Users/{id:int}/Roles")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> AssignRoles(int id, [FromBody] AssignRolesCommand command)
        {
            command.Id = id;
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPut("Users/{id:int}/Stations")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> AssignStations(int id, [FromBody] AssignStaffStationsCommand command)
        {
            command.Id = id;
            var response = await _mediator.Send(command);
            return Ok(response);
        }
    }
}
