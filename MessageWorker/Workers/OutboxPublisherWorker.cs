using MessageWorker.Abstractions.Contracts;
using MessageWorker.Domain.Entities;
using MessageWorker.Infrastructure;
using MessageWorker.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace MessageWorker.Workers
{
    public class OutboxPublisherWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IMessageBusPublisher _publisher;
        private readonly MessageBusSettings _settings;

        private readonly string _workerId = $"{Environment.MachineName}-{Guid.NewGuid():N}";

        public OutboxPublisherWorker(IServiceScopeFactory scopeFactory, IMessageBusPublisher publisher, IOptions<MessageBusSettings> options)
        {
            _scopeFactory = scopeFactory;
            _publisher = publisher;
            _settings = options.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var messages = await ClaimBatchAsync(db, batchSize: 50, lockSeconds: 60, stoppingToken);

                if (messages.Count == 0)
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(500), stoppingToken);
                    continue;
                }

                foreach (var msg in messages)
                {
                    try
                    {
                        await _publisher.PublishAsync(
                                _settings.Exchange, 
                                msg.Type, 
                                msg.Payload, 
                                options: new PublishOptions(    CorrelationId: $"outbox:{msg.Id.ToString()}",
                                                                Headers: new Dictionary<string, object?>
                                                                {
                                                                    ["source"] = "telegram-worker",
                                                                    ["message_type"] = msg.Type,
                                                                    ["occurred_on"] = msg.OccurredOnUtc.ToString("O"),
                                                                    ["retry_count"] = msg.TryCount,
                                                                    ["schema_version"] = 1
                                                                }), 
                                ct: stoppingToken);

                        msg.Status = OutboxStatus.Published;
                        msg.ProcessedOnUtc = DateTime.UtcNow;
                        msg.LastError = null;
                        msg.LockedBy = null;
                        msg.LockUntilUtc = null;
                    }
                    catch (Exception ex)
                    {
                        msg.TryCount++;
                        msg.LastError = ex.Message;

                        msg.Status = msg.TryCount >= 20 ? OutboxStatus.Failed : OutboxStatus.Pending;
                        msg.LockedBy = null;
                        msg.LockUntilUtc = null;
                    }
                }

                await db.SaveChangesAsync(stoppingToken);
            }
        }

        private async Task<List<OutboxMessage>> ClaimBatchAsync(
            AppDbContext db,
            int batchSize,
            int lockSeconds,
            CancellationToken ct)
        {
            var workerId = _workerId;
            var lockInterval = $"{lockSeconds} seconds";

            var sql = $@"
        WITH cte AS (
            SELECT id
            FROM outbox_messages
            WHERE (status = 0)
               OR (status = 1 AND lock_until_utc < NOW())
            ORDER BY created_on_utc
            FOR UPDATE SKIP LOCKED
            LIMIT {batchSize}
        )
        UPDATE outbox_messages o
        SET status = 1,
            locked_by = @workerId,
            lock_until_utc = NOW() + INTERVAL '{lockInterval}'
        FROM cte
        WHERE o.id = cte.id
        RETURNING o.*;
    ";

            return await db.OutboxMessages
                .FromSqlRaw(sql, new Npgsql.NpgsqlParameter("workerId", workerId))
                .AsTracking()
                .ToListAsync(ct);
        }

    }
}
