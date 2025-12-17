using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources.Authentication;
using Sudan_Train.Data.Entity.Identity;
using Sudan_Train.Service.Abstracts;
using Sudan_Train.Service.Models;
using System.Security.Claims;
using System.Web;

namespace Sudan_Train.Core.Features.Authentication.Commands.ChangeEmail
{
    public class ChangeEmailCommandHandler : ResponseHandler, IRequestHandler<ChangeEmailCommand, Response<string>>
    {
        private readonly UserManager<User> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStringLocalizer<AuthenticationResources> _authLocalizer;
        private readonly IEmailService _emailService;

        public ChangeEmailCommandHandler(
            UserManager<User> userManager,
            IHttpContextAccessor httpContextAccessor,
            IStringLocalizer<AuthenticationResources> authLocalizer,
            IEmailService emailService) : base(authLocalizer)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _authLocalizer = authLocalizer;
            _emailService = emailService;
        }

        public async Task<Response<string>> Handle(ChangeEmailCommand request, CancellationToken cancellationToken)
        {
            // Get current user from HttpContext
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? _httpContextAccessor.HttpContext?.User.FindFirst("uid")?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized<string>(_authLocalizer[AuthenticationResourcesKeys.UserNotFound]);
            }

            var user = await _userManager.FindByIdAsync(userIdClaim);
            if (user == null)
            {
                return NotFound<string>(_authLocalizer[AuthenticationResourcesKeys.UserNotFound]);
            }

            // Verify current password
            var isValidPassword = await _userManager.CheckPasswordAsync(user, request.CurrentPassword);
            if (!isValidPassword)
            {
                return BadRequest<string>(_authLocalizer[AuthenticationResourcesKeys.PasswordNotCorrect]);
            }

            // Check if new email is already in use
            var existingUser = await _userManager.FindByEmailAsync(request.NewEmail);
            if (existingUser != null && existingUser.Id != user.Id)
            {
                return BadRequest<string>(_authLocalizer[AuthenticationResourcesKeys.EmailAlreadyInUse]);
            }

            // Generate email change token
            var token = await _userManager.GenerateChangeEmailTokenAsync(user, request.NewEmail);

            // Create confirmation URL
            var encodedToken = HttpUtility.UrlEncode(token);
            var encodedEmail = HttpUtility.UrlEncode(request.NewEmail);
            var confirmationUrl = $"https://yourdomain.com/confirm-email-change?userId={user.Id}&token={encodedToken}&newEmail={encodedEmail}";

            // Send confirmation email to NEW email address
            var emailSubject = "Confirm Your Email Change";
            var emailBody = $@"
                <h2>Email Change Request</h2>
                <p>Hello {user.FirstName},</p>
                <p>We received a request to change your email address. Please click the link below to confirm:</p>
                <p><a href='{confirmationUrl}'>Confirm Email Change</a></p>
                <p>If you didn't request this change, please ignore this email and your email address will remain unchanged.</p>
                <p>This link will expire in 24 hours.</p>
            ";

            await _emailService.SendEmailAsync(request.NewEmail, emailSubject, emailBody, EmailSendingStrategy.Queued);

            return Success<string>(_authLocalizer[AuthenticationResourcesKeys.EmailChangeRequested]);
        }
    }
}

