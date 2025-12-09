using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sudan_Train.Service.Abstracts;
using Sudan_Train.Service.Implementations;
using Sudan_Train.Service.Models;

namespace Sudan_Train.Service
{
    public static class ModuleServiceDependencies
    {
        public static IServiceCollection AddServiceDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            // Configure Email Settings
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));

            // Register services here
            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<IAuthenticationService, AuthenticationService>();

            return services;
        }
    }
}

