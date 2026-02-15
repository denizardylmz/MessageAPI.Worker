using System;
using System.Collections.Generic;
using System.Text;

namespace MessageWorker.Application.Interfaces
{

    public interface IEventHandler<T>
    {
        Task HandleAsync(T cmd, CancellationToken ct);
    }


}
