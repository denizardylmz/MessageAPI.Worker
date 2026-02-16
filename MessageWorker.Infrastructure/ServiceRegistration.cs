using MessageWorker.Abstractions.Contracts;
using MessageWorker.Application.Interfaces;
using MessageWorker.Infrastructure.Messaging;
using MessageWorker.Infrastructure.Persistence;
using MessageWorker.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;


namespace MessageWorker.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            services.AddOptions<MessageBusSettings>().Bind(configuration.GetSection("MessageBus"));

            services.AddScoped<IShiftRepository, ShiftRepository>();

            services.AddSingleton<RabbitMqConnectionProvider>();
            services.AddSingleton<RabbitMqConsumer>();
            services.AddSingleton<IMessageBusPublisher>(sp =>
            {
                var settings = sp.GetRequiredService<IOptions<MessageBusSettings>>().Value;
                return new RabbitMqPublisher(settings);
            });

            return services;
        }
    }

}
