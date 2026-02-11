using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace MessageWorker.Infrastructure.Messaging
{
    using RabbitMQ.Client;

    public class RabbitMqConnectionProvider : IAsyncDisposable
    {
        private readonly MessageBusSettings _settings;
        private IConnection? _connection;

        public RabbitMqConnectionProvider(IOptions<MessageBusSettings> options)
        {
            _settings = options.Value;
        }

        public async Task<IConnection> GetConnectionAsync()
        {
            if (_connection is { IsOpen: true })
                return _connection;

            var factory = new ConnectionFactory
            {
                HostName = _settings.HostName,
                Port = _settings.Port,
                UserName = _settings.UserName,
                Password = _settings.Password
            };

            _connection = await factory.CreateConnectionAsync();
            return _connection;
        }

        public async ValueTask DisposeAsync()
        {
            if (_connection != null)
                await _connection.DisposeAsync();
        }
    }



}
