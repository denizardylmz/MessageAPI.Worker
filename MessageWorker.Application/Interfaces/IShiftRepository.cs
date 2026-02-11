using MessageWorker.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MessageWorker.Application.Interfaces
{
    public interface IShiftRepository
    {
        Task AddAsync(Shift shift, CancellationToken ct);
        Task<Shift?> GetActiveShiftAsync(long userId, CancellationToken ct);
    }

}
