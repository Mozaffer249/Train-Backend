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
            // Register services here
            services.AddTransient<IAuthenticationService, AuthenticationService>();

            // Register EmailServiceProxy to forward email requests to MessagingApi microservice
            services.AddHttpClient<IEmailService, EmailServiceProxy>();

            return services;
        }
    }
}

