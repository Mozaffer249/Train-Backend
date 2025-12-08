using Microsoft.Extensions.DependencyInjection;
using Sudan_Train.Service.Abstracts;
using Sudan_Train.Service.Implementations;

namespace Sudan_Train.Service
{
    public static class ModuleServiceDependencies
    {
        public static IServiceCollection AddServiceDependencies(this IServiceCollection services)
        {
            // Register services here
            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<IAuthenticationService, AuthenticationService>();

            return services;
        }
    }
}

