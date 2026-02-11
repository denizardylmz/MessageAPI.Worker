using MessageWorker.Application.Interfaces;
using MessageWorker.Infrastructure.Messaging;
using MessageWorker.Infrastructure.Persistence;
using MessageWorker.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


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

            return services;
        }
    }

}
