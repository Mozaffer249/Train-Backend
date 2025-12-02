using Microsoft.Extensions.DependencyInjection;
using Sudan_Train.Infrastructure.Abstracts;
using Sudan_Train.Infrastructure.InfrastructureBases;
using Sudan_Train.Infrastructure.Repositories;

namespace Sudan_Train.Infrastructure
{
    public static class ModuleInfrastructureDependencies
    {
        public static IServiceCollection AddInfrastructureDependencies(this IServiceCollection services)
        {
            services.AddTransient<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddTransient(typeof(IGenericRepositoryAsync<>), typeof(GenericRepositoryAsync<>));

            //views

            //Procedure

            //functions

            return services;
        }
    }
}
