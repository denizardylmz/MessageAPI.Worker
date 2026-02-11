using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace MessageWorker.Infrastructure.Messaging
{
    public class RabbitMqConsumer
    {
        private readonly RabbitMqConnectionProvider _provider;
        private readonly MessageBusSettings _settings;

        public RabbitMqConsumer(
            RabbitMqConnectionProvider provider,
            IOptions<MessageBusSettings> options)
        {
            _provider = provider;
            _settings = options.Value;
        }

        public async Task StartAsync(Func<string, Task> onMessage)
        {
            var connection = await _provider.GetConnectionAsync();
            var channel = await connection.CreateChannelAsync();

            await channel.ExchangeDeclareAsync(_settings.Exchange, ExchangeType.Topic, durable: true, autoDelete: false);
            await channel.QueueDeclareAsync(_settings.QueueName, durable: true, exclusive: false, autoDelete: false);
            await channel.QueueBindAsync(_settings.QueueName, _settings.Exchange, "shift.*");

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (sender, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                try
                {
                    await onMessage(message);
                    await channel.BasicAckAsync(ea.DeliveryTag, false);
                }
                catch
                {
                    await channel.BasicNackAsync(ea.DeliveryTag, false, true);
                }
            };

            await channel.BasicConsumeAsync(
                queue: _settings.QueueName,
                autoAck: false,
                consumer: consumer);
        }

    }


}
