using MessageWorker.Application.Interfaces;
using MessageWorker.Domain.Entities;
using MessageWorker.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace MessageWorker.Infrastructure.Persistence.Repositories
{
    public class ShiftRepository : IShiftRepository
    {
        private readonly AppDbContext _context;

        public ShiftRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Shift shift, CancellationToken ct)
        {
            var entity = new ShiftEntity
            {
                Id = shift.Id,
                UserId = shift.UserId,
                StartTime = shift.StartTime,
                EndTime = shift.EndTime
            };

            await _context.Shifts.AddAsync(entity, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task<Shift?> GetActiveShiftAsync(long userId, CancellationToken ct)
        {
            var entity = await _context.Shifts
                .Where(x => x.UserId == userId && x.EndTime == null)
                .FirstOrDefaultAsync(ct);

            if (entity == null)
                return null;

            return new Shift(entity.UserId, entity.StartTime);
        }
    }
}
