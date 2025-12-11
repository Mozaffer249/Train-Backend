using System.Linq;
using System.Net.Http.Json;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources.Authentication;
using Sudan_Train.Data.Entity.Identity;
using Sudan_Train.Service.Models;

namespace Sudan_Train.Core.Features.Authentication.Commands.Register
{
    public class RegisterCommandHandler : ResponseHandler, IRequestHandler<RegisterCommand, Response<object>>
    {
        private readonly UserManager<User> _userManager;
        private readonly IStringLocalizer<AuthenticationResources> _authLocalizer;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<RegisterCommandHandler> _logger;

        private const string MessagingApiBaseUrlKey = "MessagingApi:BaseUrl";
        private const string MessagingApiEmailEndpoint = "/api/messaging/email";

        public RegisterCommandHandler(
            UserManager<User> userManager,
            IStringLocalizer<AuthenticationResources> authLocalizer,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ILogger<RegisterCommandHandler> logger) : base(authLocalizer)
        {
            _userManager = userManager;
            _authLocalizer = authLocalizer;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<Response<object>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await ValidateUserDoesNotExist(request);
            if (validationResult != null)
                return validationResult;

            var user = await CreateUserAsync(request);
            if (user == null)
                return BadRequest<object>(_authLocalizer[AuthenticationResourcesKeys.FailedToAddUser]);

            await SendWelcomeEmailAsync(user, cancellationToken);

            return Created<object>(
                _authLocalizer[AuthenticationResourcesKeys.UserRegisteredSuccessfully],
                entity: new { });
        }

        private async Task<Response<object>?> ValidateUserDoesNotExist(RegisterCommand request)
        {
            var emailExists = await IsEmailAlreadyRegistered(request.Email!);
            if (emailExists)
                return BadRequest<object>(_authLocalizer[AuthenticationResourcesKeys.EmailIsExist]);

            if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
            {
                var phoneExists = IsPhoneNumberAlreadyRegistered(request.PhoneNumber);
                if (phoneExists)
                    return BadRequest<object>(_authLocalizer[AuthenticationResourcesKeys.PhoneNumberIsExist]);
            }

            return null;
        }

        private async Task<bool> IsEmailAlreadyRegistered(string email)
        {
            var existingUser = await _userManager.FindByEmailAsync(email);
            return existingUser != null;
        }

        private bool IsPhoneNumberAlreadyRegistered(string phoneNumber)
        {
            return _userManager.Users.Any(u => u.PhoneNumber == phoneNumber);
        }

        private async Task<User?> CreateUserAsync(RegisterCommand request)
        {
            var user = MapRequestToUser(request);
            var result = await _userManager.CreateAsync(user, request.Password!);

            return result.Succeeded ? user : null;
        }

        private User MapRequestToUser(RegisterCommand request)
        {
            var username = ExtractUsernameFromEmail(request.Email!);

            return new User
            {
                FirstName = request.FirstName!,
                LastName = request.LastName!,
                UserName = username,
                Email = request.Email!,
                PhoneNumber = request.PhoneNumber,
                IsActive = false
            };
        }

        private static string ExtractUsernameFromEmail(string email)
        {
            return email.Split('@')[0];
        }

        private async Task SendWelcomeEmailAsync(User user, CancellationToken cancellationToken)
        {
            try
            {
                var messagingApiUrl = _configuration[MessagingApiBaseUrlKey];
                if (string.IsNullOrEmpty(messagingApiUrl))
                {
                    _logger.LogWarning("MessagingApi BaseUrl not configured. Welcome email not sent.");
                    return;
                }

                var emailRequest = BuildWelcomeEmailRequest(user);
                await SendEmailRequestAsync(messagingApiUrl, emailRequest, cancellationToken);

                _logger.LogInformation("Welcome email queued successfully for {Email}", user.Email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send welcome email to {Email}", user.Email);
            }
        }

        private object BuildWelcomeEmailRequest(User user)
        {
            var fullName = $"{user.FirstName} {user.LastName}";
            var emailSubject = _authLocalizer[AuthenticationResourcesKeys.WelcomeEmailSubject].ToString();
            var emailBody = string.Format(
                _authLocalizer[AuthenticationResourcesKeys.WelcomeEmailBody],
                fullName,
                user.Email,
                user.UserName);

            return new
            {
                to = user.Email,
                subject = emailSubject,
                body = emailBody,
                isHtml = true,
                strategy = EmailSendingStrategy.Queued.ToIntValue()
            };
        }

        private async Task SendEmailRequestAsync(string baseUrl, object emailRequest, CancellationToken cancellationToken)
        {
            var httpClient = _httpClientFactory.CreateClient();
            var endpoint = $"{baseUrl}{MessagingApiEmailEndpoint}";
            var response = await httpClient.PostAsJsonAsync(endpoint, emailRequest, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "Failed to send welcome email via MessagingApi. Status: {StatusCode}",
                    response.StatusCode);
            }
        }
    }
}