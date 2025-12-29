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

            // Infrastructure Repositories
            services.AddTransient<ICityRepository, CityRepository>();
            services.AddTransient<IStationRepository, StationRepository>();
            services.AddTransient<IRouteRepository, RouteRepository>();
            services.AddTransient<IRouteStationRepository, RouteStationRepository>();
            services.AddTransient<IFareRepository, FareRepository>();
            services.AddTransient<ITrainRepository, TrainRepository>();
            services.AddTransient<ICoachRepository, CoachRepository>();
            services.AddTransient<ISeatRepository, SeatRepository>();
            services.AddTransient<ITripRepository, TripRepository>();
            services.AddTransient<ITripSeatRepository, TripSeatRepository>();

            // Database Seeder
            services.AddTransient<DatabaseSeeder>();
            services.AddTransient<RoleSeeder>();
            services.AddTransient<UserSeeder>();
            services.AddTransient<CitiesSeeder>();

            //views

            //Procedure

            //functions

            return services;
        }
    }
}
