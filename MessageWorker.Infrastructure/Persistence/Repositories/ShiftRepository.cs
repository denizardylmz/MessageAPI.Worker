using MessageWorker.Application.Interfaces;
using MessageWorker.Domain.Entities;
using MessageWorker.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
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

        public async Task UpdateAsync(Guid Id, Shift shift, CancellationToken ct)
        {

            var shiftEntity = await _context.Shifts
                .Where(x => x.Id == Id)
                .FirstOrDefaultAsync(ct);

            if (shiftEntity == null)
            {
                throw new InvalidOperationException($"Shift with Id {shift.Id} not found.");
            }

            shiftEntity.UserId = shift.UserId;
            shiftEntity.StartTime = shift.StartTime;
            shiftEntity.EndTime = shift.EndTime;

            _context.Shifts.Update(shiftEntity);
            await _context.SaveChangesAsync(ct);
        }

        public async Task<Shift?> GetActiveShiftAsync(long userId, CancellationToken ct)
        {
            var entity = await _context.Shifts
                .Where(x => x.UserId == userId && x.EndTime == null)
                .AsNoTracking()
                .FirstOrDefaultAsync(ct);

            if (entity == null)
                return null;

            return new Shift(entity.Id, entity.UserId, entity.StartTime);
        }
    }
}
