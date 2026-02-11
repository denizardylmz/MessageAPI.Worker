using MessageWorker.Application.Features.Shifts;
using MessageWorker.Infrastructure.Messaging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace MessageWorker.Workers
{
    public class RabbitMqConsumerWorker : BackgroundService
    {
        private readonly RabbitMqConsumer _consumer;
        private readonly IServiceScopeFactory _scopeFactory;

        public RabbitMqConsumerWorker(
            RabbitMqConsumer consumer,
            IServiceScopeFactory scopeFactory)
        {
            _consumer = consumer;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _consumer.StartAsync(async (message) =>
            { 
                using var scope = _scopeFactory.CreateScope();

                var handler = scope.ServiceProvider
                    .GetRequiredService<StartShiftHandler>();

                var command = JsonSerializer.Deserialize<ShiftCommand>(
                    message,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                await handler.Handle(command!, stoppingToken);
            });

        }
    }

}
