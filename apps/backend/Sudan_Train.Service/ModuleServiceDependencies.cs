using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sudan_Train.Service.Abstracts;
using Sudan_Train.Service.Implementations;

namespace Sudan_Train.Service
{
    public static class ModuleServiceDependencies
    {
        public static IServiceCollection AddServiceDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            // Authentication & Security services
            services.AddTransient<IAuthenticationService, AuthenticationService>();
            services.AddTransient<ITwoFactorAuthenticationService, TwoFactorAuthenticationService>();
            services.AddSingleton<IRateLimitingService, RateLimitingService>();
            services.AddTransient<IAuditService, AuditService>();
            services.AddTransient<ISessionManagementService, SessionManagementService>();
            services.AddTransient<IPasswordSecurityService, PasswordSecurityService>();
            services.AddTransient<ISecurityNotificationService, SecurityNotificationService>();
            services.AddTransient<IRiskAssessmentService, RiskAssessmentService>();

            // Infrastructure services
            services.AddTransient<IGeographyService, GeographyService>();
            services.AddTransient<IStationService, StationService>();
            services.AddTransient<IRouteService, RouteService>();
            services.AddTransient<ITrainService, TrainService>();
            services.AddTransient<ITripService, TripService>();

            // Register EmailServiceProxy to forward email requests to MessagingApi microservice
            services.AddHttpClient<IEmailService, EmailServiceProxy>();

            // Add memory cache for rate limiting and IP blocking
            services.AddMemoryCache();

            return services;
        }
    }
}


