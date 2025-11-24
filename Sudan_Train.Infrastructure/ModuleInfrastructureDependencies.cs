using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sudan_Train.Infrastructure
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