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

            // Configure RabbitMQ Settings
            services.Configure<RabbitMQSettings>(configuration.GetSection("RabbitMQSettings"));

            // Register message queue service
            services.AddSingleton<IMessageQueueService, RabbitMQService>();

            // Register services here
            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<IAuthenticationService, AuthenticationService>();

            // Register background email consumer
            services.AddHostedService<EmailConsumerService>();

            return services;
        }
    }
}

