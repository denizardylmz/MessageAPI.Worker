using MessageWorker.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace MessageWorker.Domain.DomainEvents
{
    public sealed record ShiftEndedEvent(Guid ShiftId, long TelegramUserId, DateTime EndTimeUtc ) : IDomainEvent
    {
        public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
    }
}
