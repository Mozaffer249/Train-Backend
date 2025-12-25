using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Sudan_Train.Core.Behaviors;
using Sudan_Train.Core.Services.Google;
using Sudan_Train.Core.Services.Seeding;
using Sudan_Train.Core.Services.Spatial;
using System.Reflection;

namespace Sudan_Train.Core
{
    public static class ModuleCoreDependencies
    {
        public static IServiceCollection AddCoreDependencies(this IServiceCollection services)
        {
            // Register MediatR
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));

            // Register AutoMapper
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            // Register FluentValidation validators
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            // Register Validation Behavior
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            // Register Google Integration services
            services.AddHttpClient<IGoogleGeocodingService, GoogleGeocodingService>();
            services.AddHttpClient<IGooglePlacesService, GooglePlacesService>();

            // Register Spatial Validation service
            services.AddTransient<ISpatialValidationService, SpatialValidationService>();

            // Register Spatial Utility service
            services.AddSingleton<SpatialUtilityService>();

            return services;
        }
    }
}

