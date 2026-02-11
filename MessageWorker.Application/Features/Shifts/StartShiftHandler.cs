using MessageWorker.Application.Interfaces;
using MessageWorker.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MessageWorker.Application.Features.Shifts
{
    public class StartShiftHandler
    {
        private readonly IShiftRepository _repository;

        public StartShiftHandler(IShiftRepository repository)
        {
            _repository = repository;
        }

        public async Task Handle(ShiftCommand cmd, CancellationToken ct)
        {
            var existingShift = await _repository.GetActiveShiftAsync(cmd.telegramUserId, ct);

            if (existingShift is not null)
                throw new Exception("Active shift already exists.");

            var shift = new Shift(cmd.telegramUserId, cmd.date);

            await _repository.AddAsync(shift, ct);
        }
    }

}
