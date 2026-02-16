using System;
using System.Collections.Generic;
using System.Text;

namespace MessageWorker.Domain.Abstractions
{
    public interface IDomainEvent
    {
        DateTime OccurredOnUtc { get; }
    }
}
