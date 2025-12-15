using System.Linq;
using System.Net.Http.Json;
using System.Web;
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

            // Generate confirmation token
            var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            // Send confirmation email (instead of welcome email)
            await SendConfirmationEmailAsync(user, confirmationToken, cancellationToken);

            return Created<object>(
                _authLocalizer[AuthenticationResourcesKeys.UserRegisteredSuccessfully],
                entity: new { Message = "Please check your email to confirm your account." });
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

        private async Task SendConfirmationEmailAsync(User user, string token, CancellationToken cancellationToken)
        {
            try
            {
                var messagingApiUrl = _configuration[MessagingApiBaseUrlKey];
                if (string.IsNullOrEmpty(messagingApiUrl))
                {
                    _logger.LogWarning("MessagingApi BaseUrl not configured. Confirmation email not sent.");
                    return;
                }

                var emailRequest = BuildConfirmationEmailRequest(user, token);
                await SendEmailRequestAsync(messagingApiUrl, emailRequest, cancellationToken);

                _logger.LogInformation("Confirmation email queued successfully for {Email}. User ID: {UserId}, Token: {Token}",
                    user.Email, user.Id, token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send confirmation email to {Email}", user.Email);
            }
        }

        private object BuildConfirmationEmailRequest(User user, string token)
        {
            var encodedToken = HttpUtility.UrlEncode(token);
            var encodedUserId = user.Id;

            // Frontend confirmation URL
            // Development: http://localhost:3000/confirm-email
            // Production: https://yourdomain.com/confirm-email
            var frontendBaseUrl = _configuration["Frontend:BaseUrl"] ?? "http://localhost:3000";
            var confirmationUrl = $"{frontendBaseUrl}/confirm-email?userId={encodedUserId}&code={encodedToken}";

            var emailSubject = "Confirm Your Email - Sudan Train";
            var emailBody = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <style>
        body {{
            font-family: Arial, sans-serif;
            line-height: 1.6;
            color: #333;
            background-color: #f4f4f4;
            margin: 0;
            padding: 0;
        }}
        .container {{
            max-width: 600px;
            margin: 30px auto;
            background: #ffffff;
            border-radius: 8px;
            overflow: hidden;
            box-shadow: 0 2px 10px rgba(0,0,0,0.1);
        }}
        .header {{
            background: linear-gradient(135deg, #007bff 0%, #0056b3 100%);
            color: white;
            padding: 30px;
            text-align: center;
        }}
        .header h1 {{
            margin: 0;
            font-size: 28px;
            font-weight: bold;
        }}
        .content {{
            padding: 40px 30px;
        }}
        .content h2 {{
            color: #007bff;
            margin-top: 0;
            font-size: 24px;
        }}
        .content p {{
            margin: 15px 0;
            font-size: 16px;
        }}
        .button-container {{
            text-align: center;
            margin: 35px 0;
        }}
        .confirm-button {{
            display: inline-block;
            background: #007bff;
            color: #ffffff !important;
            padding: 15px 40px;
            text-decoration: none;
            border-radius: 5px;
            font-weight: bold;
            font-size: 16px;
            box-shadow: 0 4px 6px rgba(0,123,255,0.3);
            transition: background 0.3s ease;
        }}
        .confirm-button:hover {{
            background: #0056b3;
        }}
        .link-section {{
            background: #f8f9fa;
            padding: 20px;
            border-radius: 5px;
            margin: 25px 0;
            border-left: 4px solid #007bff;
        }}
        .link-section p {{
            margin: 5px 0;
            font-size: 14px;
            color: #666;
        }}
        .link-text {{
            word-break: break-all;
            color: #007bff;
            font-size: 13px;
        }}
        .footer {{
            background: #f8f9fa;
            padding: 25px 30px;
            text-align: center;
            border-top: 1px solid #e9ecef;
        }}
        .footer p {{
            margin: 8px 0;
            font-size: 13px;
            color: #6c757d;
        }}
        .warning {{
            background: #fff3cd;
            border-left: 4px solid #ffc107;
            padding: 15px;
            margin: 20px 0;
            border-radius: 4px;
        }}
        .warning p {{
            margin: 5px 0;
            font-size: 14px;
            color: #856404;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>🚂 Sudan Train</h1>
        </div>
        
        <div class='content'>
            <h2>Welcome, {user.FirstName}!</h2>
            <p>Thank you for registering with Sudan Train. We're excited to have you on board!</p>
            <p>To complete your registration and activate your account, please confirm your email address by clicking the button below:</p>
            
            <div class='button-container'>
                <a href='{confirmationUrl}' class='confirm-button'>Confirm Email Address</a>
            </div>
            
            <div class='link-section'>
                <p><strong>Can't click the button?</strong></p>
                <p>Copy and paste this link into your browser:</p>
                <p class='link-text'>{confirmationUrl}</p>
            </div>
            
            <div class='warning'>
                <p><strong>⏰ Important:</strong> This confirmation link will expire in 24 hours.</p>
            </div>
            
            <p>Once confirmed, you'll be able to:</p>
            <ul>
                <li>Book train tickets online</li>
                <li>Manage your travel history</li>
                <li>Receive booking updates and notifications</li>
                <li>Access exclusive member benefits</li>
            </ul>
        </div>
        
        <div class='footer'>
            <p><strong>Didn't create an account?</strong></p>
            <p>If you didn't sign up for Sudan Train, please ignore this email. Your email address will not be used.</p>
            <hr style='border: none; border-top: 1px solid #e9ecef; margin: 15px 0;'>
            <p>© 2024 Sudan Train. All rights reserved.</p>
            <p>This is an automated message, please do not reply to this email.</p>
        </div>
    </div>
</body>
</html>";

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