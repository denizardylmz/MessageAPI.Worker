using MessageWorker.Application.Interfaces;
using MessageWorker.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MessageWorker.Application.Features.Shifts
{
    public class StartShiftHandler : IEventHandler<ShiftCommand>
    {
        private readonly IShiftRepository _repository;

        public StartShiftHandler(IShiftRepository repository)
        {
            _repository = repository;
        }

        public async Task HandleAsync(ShiftCommand cmd, CancellationToken ct)
        {
            var existingShift = await _repository.GetActiveShiftAsync(cmd.telegramUserId, ct);

            if (existingShift is not null)
                throw new Exception("Active shift already exists.");

            var shift = new Shift(cmd.telegramUserId, cmd.date);

            await _repository.AddAsync(shift, ct);
        }
    }

    public class EndShiftHandler : IEventHandler<ShiftCommand>
    {
        private readonly IShiftRepository _repository;

        public EndShiftHandler(IShiftRepository repository)
        {
            _repository = repository;
        }

        public async Task HandleAsync(ShiftCommand cmd, CancellationToken ct)
        {
            var shift = await _repository.GetActiveShiftAsync(cmd.telegramUserId, ct);

            if (shift == null)
                throw new Exception("No shift-on");
            
            shift.End(cmd.date);
            await _repository.UpdateAsync(shift.Id, shift, ct);
        }
    }

}
