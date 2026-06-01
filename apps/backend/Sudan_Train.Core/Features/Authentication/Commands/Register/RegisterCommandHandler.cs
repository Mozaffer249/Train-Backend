using System;
using System.Linq;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources.Authentication;
using Sudan_Train.Data.AppMetaData;
using Sudan_Train.Data.Entity.Identity;
using Sudan_Train.Infrastructure.context;
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
        private readonly ApplicationDBContext _context;

        private const string MessagingApiBaseUrlKey = "MessagingApi:BaseUrl";
        private const string MessagingApiEmailEndpoint = "/api/messaging/email";

        public RegisterCommandHandler(
            UserManager<User> userManager,
            IStringLocalizer<AuthenticationResources> authLocalizer,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ILogger<RegisterCommandHandler> logger,
            ApplicationDBContext context) : base(authLocalizer)
        {
            _userManager = userManager;
            _authLocalizer = authLocalizer;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;
            _context = context;
        }

        public async Task<Response<object>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            // If a user with this email already exists, branch on confirmation status:
            //   • confirmed/active → real "email taken" error
            //   • unconfirmed     → treat as a resume: regenerate the OTP, resend it,
            //                       and return the same success shape so the client lands
            //                       on the confirm page.
            var existingUser = await _userManager.FindByEmailAsync(request.Email!);
            if (existingUser != null)
            {
                if (existingUser.EmailConfirmed || existingUser.IsActive)
                {
                    return BadRequest<object>(_authLocalizer[AuthenticationResourcesKeys.EmailIsExist]);
                }

                var resendOtp = GenerateOtpCode();
                await StoreOtpInDatabaseAsync(existingUser.Id, resendOtp);
                await SendConfirmationEmailAsync(existingUser, resendOtp, cancellationToken);

                return Created<object>(
                    _authLocalizer[AuthenticationResourcesKeys.UserRegisteredSuccessfully],
                    entity: new
                    {
                        Message = "A new confirmation code has been sent to your email.",
                        UserId = existingUser.Id,
                        Email = existingUser.Email,
                        ResumeConfirmation = true,
                    });
            }

            if (!string.IsNullOrWhiteSpace(request.PhoneNumber) &&
                IsPhoneNumberAlreadyRegistered(request.PhoneNumber))
            {
                return BadRequest<object>(_authLocalizer[AuthenticationResourcesKeys.PhoneNumberIsExist]);
            }

            var user = await CreateUserAsync(request);
            if (user == null)
                return BadRequest<object>(_authLocalizer[AuthenticationResourcesKeys.FailedToAddUser]);

            // Generate 4-digit OTP
            var otpCode = GenerateOtpCode();
            await StoreOtpInDatabaseAsync(user.Id, otpCode);

            // Send confirmation email with OTP
            await SendConfirmationEmailAsync(user, otpCode, cancellationToken);

            return Created<object>(
                _authLocalizer[AuthenticationResourcesKeys.UserRegisteredSuccessfully],
                entity: new
                {
                    Message = "Please check your email for your confirmation code.",
                    UserId = user.Id,
                    Email = user.Email,
                });
        }

        private bool IsPhoneNumberAlreadyRegistered(string phoneNumber)
        {
            return _userManager.Users.Any(u => u.PhoneNumber == phoneNumber);
        }

        private async Task<User?> CreateUserAsync(RegisterCommand request)
        {
            var user = MapRequestToUser(request);
            var result = await _userManager.CreateAsync(user, request.Password!);
            if (!result.Succeeded)
                return null;

            // Self-registered users are always Customers. Ignore role-assignment errors
            // (e.g. role missing) so registration still succeeds; the user just has no role.
            var roleResult = await _userManager.AddToRoleAsync(user, Roles.Customer);
            if (!roleResult.Succeeded)
            {
                _logger.LogWarning(
                    "User {UserId} created but Customer role assignment failed: {Errors}",
                    user.Id, string.Join(", ", roleResult.Errors));
            }

            return user;
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

        private string GenerateOtpCode()
        {
            var random = new Random();
            return random.Next(1000, 9999).ToString(); // Generates 1000-9999
        }

        private async Task StoreOtpInDatabaseAsync(int userId, string otpCode)
        {
            // Delete any existing OTPs for this user
            var existingOtps = _context.EmailConfirmationOtps
                .Where(o => o.UserId == userId);
            _context.EmailConfirmationOtps.RemoveRange(existingOtps);

            // Create new OTP
            var otp = new EmailConfirmationOtp
            {
                UserId = userId,
                OtpCode = otpCode,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMinutes(5), // 5 minute expiry
                IsUsed = false
            };

            _context.EmailConfirmationOtps.Add(otp);
            await _context.SaveChangesAsync();
        }

        private async Task SendConfirmationEmailAsync(User user, string otpCode, CancellationToken cancellationToken)
        {
            try
            {
                var messagingApiUrl = _configuration[MessagingApiBaseUrlKey];
                if (string.IsNullOrEmpty(messagingApiUrl))
                {
                    _logger.LogWarning("MessagingApi BaseUrl not configured. Confirmation email not sent.");
                    return;
                }

                var emailRequest = BuildConfirmationEmailRequest(user, otpCode);
                await SendEmailRequestAsync(messagingApiUrl, emailRequest, cancellationToken);

                _logger.LogInformation("Confirmation email queued for {Email}. User ID: {UserId}, OTP: {OtpCode}",
                    user.Email, user.Id, otpCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send confirmation email to {Email}", user.Email);
            }
        }

        private object BuildConfirmationEmailRequest(User user, string otpCode)
        {
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
        .otp-code {{
            font-size: 48px;
            font-weight: bold;
            color: #007bff;
            text-align: center;
            letter-spacing: 15px;
            padding: 30px;
            background: #f8f9fa;
            border-radius: 10px;
            margin: 30px 0;
            border: 2px dashed #007bff;
        }}
        .info-section {{
            background: #f8f9fa;
            padding: 20px;
            border-radius: 5px;
            margin: 25px 0;
            border-left: 4px solid #007bff;
        }}
        .info-section p {{
            margin: 5px 0;
            font-size: 14px;
            color: #666;
        }}
        .info-section strong {{
            color: #007bff;
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
        .steps {{
            background: #e7f3ff;
            padding: 20px;
            border-radius: 5px;
            margin: 20px 0;
        }}
        .steps ol {{
            margin: 10px 0;
            padding-left: 20px;
        }}
        .steps li {{
            margin: 8px 0;
            font-size: 15px;
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
            <p>Thank you for registering with Sudan Train.</p>
            <p>Your email confirmation code is:</p>
            
            <div class='otp-code'>{otpCode}</div>
            
            <div class='warning'>
                <p><strong>⏰ Important:</strong> This code will expire in 5 minutes.</p>
            </div>
            
            <div class='info-section'>
                <p><strong>Your Email:</strong> {user.Email}</p>
            </div>

            <div class='steps'>
                <p><strong>How to confirm your email:</strong></p>
                <ol>
                    <li>Return to the confirmation page in the app</li>
                    <li>Enter the 4-digit code shown above</li>
                    <li>Submit to activate your account</li>
                </ol>
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
            <p>If you didn't sign up for Sudan Train, please ignore this email.</p>
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