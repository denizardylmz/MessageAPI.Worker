using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace MessageWorker.Domain.Entities
{
    public class OutboxMessage
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public DateTime OccurredOnUtc { get; set; }
        public DateTime CreatedOnUtc { get; set; } = DateTime.UtcNow;

        public string Type { get; set; } = default!;
        public JsonDocument Payload { get; set; } = default!;

        public OutboxStatus Status { get; set; } = OutboxStatus.Pending;

        public string? LockedBy { get; set; }
        public DateTime? LockUntilUtc { get; set; }

        public int TryCount { get; set; }
        public string? LastError { get; set; }

        public DateTime? ProcessedOnUtc { get; set; }
    }


    public enum OutboxStatus
    {
        Pending = 0,
        Processing = 1,
        Published = 2,
        Failed = 3
    }
}
