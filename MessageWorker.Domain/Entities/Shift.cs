using System;
using System.Collections.Generic;
using System.Text;

namespace MessageWorker.Domain.Entities
{
    public class Shift
    {
        public Guid Id { get; private set; }
        public long UserId { get; private set; }
        public DateTime StartTime { get; private set; }
        public DateTime? EndTime { get; private set; }

        private Shift() { }

        public Shift(long userId, DateTime startTime)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            StartTime = startTime;
        }

        public void End(DateTime endTime)
        {
            if (endTime < StartTime)
                throw new InvalidOperationException("Invalid shift end time");

            EndTime = endTime;
        }
    }

}
