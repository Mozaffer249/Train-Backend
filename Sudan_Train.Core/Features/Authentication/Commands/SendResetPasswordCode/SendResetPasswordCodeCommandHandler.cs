using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources;
using Sudan_Train.Data.Entity.Identity;
using Sudan_Train.Service.Abstracts;

namespace Sudan_Train.Core.Features.Authentication.Commands.SendResetPasswordCode
{
    public class SendResetPasswordCodeCommandHandler : ResponseHandler, IRequestHandler<SendResetPasswordCodeCommand, Response<string>>
    {
        private readonly UserManager<User> _userManager;
        private readonly IEmailService _emailService;

        public SendResetPasswordCodeCommandHandler(
            IStringLocalizer<SharedResources> stringLocalizer,
            UserManager<User> userManager,
            IEmailService emailService) : base(stringLocalizer)
        {
            _userManager = userManager;
            _emailService = emailService;
        }

        public async Task<Response<string>> Handle(SendResetPasswordCodeCommand request, CancellationToken cancellationToken)
        {
            // Check if user exists
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
            {
                return NotFound<string>(_stringLocalizer[SharedResourcesKeys.EmailIsNotExist]);
            }

            if (!user.IsActive)
            {
                return Unauthorized<string>(_stringLocalizer[SharedResourcesKeys.UserIsNotActive]);
            }

            // Generate password reset token
            var resetCode = await _userManager.GeneratePasswordResetTokenAsync(user);

            // Send email with reset code
            var emailMessage = $"Your password reset code is: {resetCode}";

            await _emailService.SendEmailAsync(
                user.Email!,
                "Password Reset Code - Train Booking System",
                emailMessage);

            return Success<string>(_stringLocalizer[SharedResourcesKeys.Success]);
        }
    }
}
