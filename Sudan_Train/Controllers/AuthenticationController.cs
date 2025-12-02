using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Sudan_Train.Base;
using Sudan_Train.Core.Features.Authentication.Commands.Login;
using Sudan_Train.Core.Features.Authentication.Commands.Register;
using Sudan_Train.Core.Wrappers;
using Sudan_Train.Data.AppMetaData;
using Sudan_Train.Data.Results;
using System.Net;

namespace Sudan_Train.Controllers
{
	public class AuthenticationController : AppControllerBase
	{
		/// <summary>
		/// Register a new user
		/// </summary>
		/// <param name="command">Registration details</param>
		/// <returns>Registration result</returns>
		[HttpPost(Router.AuthenticationRegister)]
		public async Task<IActionResult> Register([FromBody] RegisterCommand command)
		{
			try
			{
				var response = await Mediator.Send(command);

				if (response.Succeeded)
				{
					response.StatusCode = HttpStatusCode.Created;
				}
				else
				{
					response.StatusCode = HttpStatusCode.BadRequest;
				}

				return NewResult(response);
			}
			catch (ValidationException ex)
			{
				var response = new Response<string>(ex.Message)
				{
					StatusCode = HttpStatusCode.BadRequest,
					Errors = new List<string> { ex.Message }
				};
				return NewResult(response);
			}
		}

		/// <summary>
		/// Login with username and password
		/// </summary>
		/// <param name="command">Login credentials</param>
		/// <returns>JWT authentication result</returns>
		[HttpPost(Router.AuthenticationLogin)]
		public async Task<IActionResult> Login([FromBody] LoginCommand command)
		{
			try
			{
				var response = await Mediator.Send(command);

				if (response.Succeeded)
				{
					response.StatusCode = HttpStatusCode.OK;
				}
				else
				{
					response.StatusCode = HttpStatusCode.Unauthorized;
				}

				return NewResult(response);
			}
			catch (ValidationException ex)
			{
				var response = new Response<JwtAuthResult>(ex.Message)
				{
					StatusCode = HttpStatusCode.BadRequest,
					Errors = new List<string> { ex.Message }
				};
				return NewResult(response);
			}
		}
	}
}

