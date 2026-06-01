using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources.Authentication;
using Sudan_Train.Data.Entity.Identity;
using Sudan_Train.Infrastructure.InfrastructureBases;
using Sudan_Train.Service.Abstracts;
using System.Security.Claims;

namespace Sudan_Train.Core.Features.Authentication.Queries.ExportUserData
{
    public class ExportUserDataQueryHandler : ResponseHandler, IRequestHandler<ExportUserDataQuery, Response<UserDataExport>>
    {
        private readonly UserManager<User> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStringLocalizer<AuthenticationResources> _authLocalizer;
        private readonly ISessionManagementService _sessionService;
        private readonly IAuditService _auditService;
        private readonly IGenericRepositoryAsync<Data.Entity.Booking> _bookingRepository;

        public ExportUserDataQueryHandler(
            UserManager<User> userManager,
            IHttpContextAccessor httpContextAccessor,
            IStringLocalizer<AuthenticationResources> authLocalizer,
            ISessionManagementService sessionService,
            IAuditService auditService,
            IGenericRepositoryAsync<Data.Entity.Booking> bookingRepository) : base(authLocalizer)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _authLocalizer = authLocalizer;
            _sessionService = sessionService;
            _auditService = auditService;
            _bookingRepository = bookingRepository;
        }

        public async Task<Response<UserDataExport>> Handle(ExportUserDataQuery request, CancellationToken cancellationToken)
        {
            // User-data export temporarily disabled — depends on LoginSession/AuditLog tables dropped (DropAdvancedSecurityTables migration).
            await Task.CompletedTask;
            return BadRequest<UserDataExport>("User-data export is temporarily disabled.");

            /* Original implementation preserved for restoration:
            // Get current user from HttpContext
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? _httpContextAccessor.HttpContext?.User.FindFirst("uid")?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized<UserDataExport>(_authLocalizer[AuthenticationResourcesKeys.UserNotFound]);
            }

            var user = await _userManager.FindByIdAsync(userIdClaim);
            if (user == null)
            {
                return NotFound<UserDataExport>(_authLocalizer[AuthenticationResourcesKeys.UserNotFound]);
            }

            // Gather all user data
            var userDataExport = new UserDataExport
            {
                User = new UserInfo
                {
                    UserName = user.UserName!,
                    Email = user.Email!,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Address = user.Address,
                    Nationality = user.Nationality,
                    PhoneNumber = user.PhoneNumber,
                    PasswordChangedAt = user.PasswordChangedAt,
                    TwoFactorEnabled = user.TwoFactorEnabled
                }
            };

            // Get bookings
            var bookings = await _bookingRepository.GetTableNoTracking()
                .Where(b => b.UserId == userId)
                .Select(b => new BookingInfo
                {
                    BookingId = b.Id,
                    BookingDate = b.CreatedAt,
                    Status = b.Status.ToString(),
                    TotalPrice = b.TotalAmount
                })
                .ToListAsync();
            userDataExport.Bookings = bookings;

            // Get sessions
            var sessions = await _sessionService.GetActiveSessionsAsync(userId);
            userDataExport.Sessions = sessions.Select(s => new SessionInfo
            {
                DeviceInfo = s.DeviceName,
                IpAddress = s.IpAddress,
                LoginTime = s.LoginTime
            }).ToList();

            // Get audit logs (limited to last 100)
            var auditLogs = await _auditService.GetUserAuditLogsAsync(userId, 1, 100);
            userDataExport.AuditLogs = auditLogs.Select(a => new AuditLogInfo
            {
                Action = a.Action,
                Timestamp = a.Timestamp,
                IpAddress = a.IpAddress,
                Success = a.Success
            }).ToList();

            return Success<UserDataExport>(entity: userDataExport);
            */
        }
    }
}

