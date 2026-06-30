using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Features.Users.Commands.AssignRoles;
using Sudan_Train.Core.Features.Users.Commands.CreateUser;
using Sudan_Train.Core.Features.Users.Commands.SetUserActive;
using Sudan_Train.Core.Features.Users.Commands.UpdateUser;
using Sudan_Train.Core.Features.Users.Queries.GetUserList;
using Sudan_Train.Core.Filters;
using Sudan_Train.Data.AppMetaData;

namespace Sudan_Train.Controllers.Admin
{
    /// <summary>
    /// SuperAdmin-only endpoints for managing Admin and SuperAdmin accounts.
    /// </summary>
    [ApiController]
    [Route(Router.Admin)]
    [Authorize(Roles = Roles.SuperAdmin)]
    public class AdminsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AdminsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("Admins")]
        public async Task<IActionResult> List(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? search = null,
            [FromQuery] bool? isActive = null)
        {
            var query = new GetUserListQuery
            {
                Filter = new PaginatedListFilter { PageNumber = pageNumber, PageSize = pageSize, Search = search },
                IsActive = isActive,
                PrivilegedOnly = true,
            };
            var response = await _mediator.Send(query);
            return Ok(response);
        }

        [HttpPost("Admins")]
        public async Task<IActionResult> Create([FromBody] CreateUserCommand command)
        {
            if (command.Roles.Count == 0)
                command.Roles = new List<string> { Roles.Admin };

            if (!RoleHierarchy.ContainsOnlyPrivilegedRoles(command.Roles))
            {
                return BadRequest(new Response<string>
                {
                    Succeeded = false,
                    Message = "Admin accounts must be created with Admin or SuperAdmin roles only.",
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                });
            }

            var response = await _mediator.Send(command);
            return response.Succeeded ? Ok(response) : BadRequest(response);
        }

        [HttpPut("Admins/{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateUserCommand command)
        {
            command.Id = id;
            var response = await _mediator.Send(command);
            return response.Succeeded ? Ok(response) : BadRequest(response);
        }

        [HttpPut("Admins/{id:int}/Active")]
        public async Task<IActionResult> SetActive(int id, [FromBody] SetUserActiveCommand command)
        {
            command.Id = id;
            var response = await _mediator.Send(command);
            return response.Succeeded ? Ok(response) : BadRequest(response);
        }

        [HttpPut("Admins/{id:int}/Roles")]
        public async Task<IActionResult> AssignRoles(int id, [FromBody] AssignRolesCommand command)
        {
            command.Id = id;
            var response = await _mediator.Send(command);
            return response.Succeeded ? Ok(response) : BadRequest(response);
        }
    }
}
