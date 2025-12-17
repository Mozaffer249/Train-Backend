using Microsoft.Extensions.DependencyInjection;
using Sudan_Train.Infrastructure.Abstracts;
using Sudan_Train.Infrastructure.InfrastructureBases;
using Sudan_Train.Infrastructure.Repositories;
using Sudan_Train.Infrastructure.Seeder;

namespace Sudan_Train.Infrastructure
{
    public static class ModuleInfrastructureDependencies
    {
        public static IServiceCollection AddInfrastructureDependencies(this IServiceCollection services)
        {
            services.AddTransient<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddTransient(typeof(IGenericRepositoryAsync<>), typeof(GenericRepositoryAsync<>));

            // Database Seeder
            services.AddTransient<DatabaseSeeder>();
            services.AddTransient<RoleSeeder>();
            services.AddTransient<UserSeeder>();
            services.AddTransient<StateAndCitySeeder>();

            //views

            //Procedure

            //functions

            return services;
        }
    }
}
