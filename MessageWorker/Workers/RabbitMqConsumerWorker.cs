using MessageWorker.Application.Features.Shifts;
using MessageWorker.Application.Interfaces;
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

        private static readonly Dictionary<string, Type> HandlerMap = new()
        {
            ["shift.started"] = typeof(StartShiftHandler),
            ["shift.end"] = typeof(EndShiftHandler),
        };

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
            {
                await _consumer.StartAsync(async (message, routingKey) =>
                { 
                    using var scope = _scopeFactory.CreateScope();

                    if (!HandlerMap.TryGetValue(routingKey, out var handlerType))
                        throw new InvalidOperationException($"Unknown routing key: {routingKey}");

                    var handler = (IEventHandler<ShiftCommand>)scope.ServiceProvider.GetRequiredService(handlerType);

                    var cmd = JsonSerializer.Deserialize<ShiftCommand>(message,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;

                    await handler.HandleAsync(cmd, stoppingToken);

                });

            }
        }

    }

