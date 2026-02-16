using MessageWorker.Domain.Abstractions;
using MessageWorker.Domain.DomainEvents;
using System;
using System.Collections.Generic;
using System.Text;

namespace MessageWorker.Domain.Entities
{
    public class Shift
    {

        private Shift() { }

        public Shift(long userId, DateTime startTime) : this(Guid.NewGuid(), userId, startTime)
        {
        }
    
        public Shift(Guid Id, long userId, DateTime startTime)
        {
            this.Id = Id;
            UserId = userId;
            StartTime = startTime;
        }


        public Guid Id { get; private set; }
        public long UserId { get; private set; }
        public DateTime StartTime { get; private set; }
        public DateTime? EndTime { get; private set; }

        private readonly List<IDomainEvent> _domainEvents = new();
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents;

        public void End(DateTime endTime)
        {
            if (endTime < StartTime)
                throw new InvalidOperationException("Invalid shift end time");

            EndTime = endTime;

            _domainEvents.Add(new ShiftEndedEvent(Id, UserId, endTime));
        }

        public void ClearDomainEvents() => _domainEvents.Clear();

    }

}
