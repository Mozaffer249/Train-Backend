using Microsoft.Extensions.DependencyInjection;

namespace Sudan_Train.Service
{
    public static class ModuleServiceDependencies
    {
        public static IServiceCollection AddServiceDependencies(this IServiceCollection services)
        {
            // Register services here
            // Example: services.AddTransient<IAuthenticationService, AuthenticationService>();
            
            return services;
        }
    }
}

