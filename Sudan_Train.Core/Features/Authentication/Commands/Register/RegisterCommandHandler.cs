using System.Linq;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources.Authentication;
using Sudan_Train.Data.Entity.Identity;
using Sudan_Train.Service.Abstracts;

namespace Sudan_Train.Core.Features.Authentication.Commands.Register
{
    public class RegisterCommandHandler : ResponseHandler, IRequestHandler<RegisterCommand, Response<object>>
    {
        private readonly UserManager<User> _userManager;
        private readonly IStringLocalizer<AuthenticationResources> _authLocalizer;
        private readonly IEmailService _emailService;
        private readonly ILogger<RegisterCommandHandler> _logger;

        public RegisterCommandHandler(
            UserManager<User> userManager,
            IStringLocalizer<AuthenticationResources> authLocalizer,
            IEmailService emailService,
            ILogger<RegisterCommandHandler> logger) : base(authLocalizer)
        {
            _userManager = userManager;
            _authLocalizer = authLocalizer;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<Response<object>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            // Check if email already exists (validation ensures Email is not null)
            var existingEmail = await _userManager.FindByEmailAsync(request.Email!);
            if (existingEmail != null)
            {
                return BadRequest<object>(_authLocalizer[AuthenticationResourcesKeys.EmailIsExist]);
            }

            // Check if phone number already exists (if provided)
            if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
            {
                var existingPhone = _userManager.Users.Any(u => u.PhoneNumber == request.PhoneNumber);
                if (existingPhone)
                {
                    return BadRequest<object>(_authLocalizer[AuthenticationResourcesKeys.PhoneNumberIsExist]);
                }
            }

            // Create new user (validation ensures all required fields are not null)
            var user = new User
            {
                FirstName = request.FirstName!,
                LastName = request.LastName!,
                UserName = request.Email!.Split('@')[0],
                Email = request.Email!,
                PhoneNumber = request.PhoneNumber,
                IsActive = false
            };
            var result = await _userManager.CreateAsync(user, request.Password!);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return BadRequest<object>(_authLocalizer[AuthenticationResourcesKeys.FailedToAddUser]);
            }

            // Send welcome email
            try
            {
                var emailSubject = _authLocalizer[AuthenticationResourcesKeys.WelcomeEmailSubject];
                var emailBody = string.Format(
                    _authLocalizer[AuthenticationResourcesKeys.WelcomeEmailBody],
                    $"{user.FirstName} {user.LastName}",
                    user.Email,
                    user.UserName);

                await _emailService.SendEmailAsync(user.Email!, emailSubject, emailBody);
            }
            catch (Exception ex)
            {
                // Log error but don't fail registration if email fails
                // User is already created successfully
                _logger.LogError(ex, "Failed to send welcome email to {Email} for user {UserId}", user.Email, user.Id);
            }

            return Created<object>(
                _authLocalizer[AuthenticationResourcesKeys.UserRegisteredSuccessfully],
                entity: new { user.Id, user.UserName, user.Email, user.FirstName, user.LastName });
        }
    }
}