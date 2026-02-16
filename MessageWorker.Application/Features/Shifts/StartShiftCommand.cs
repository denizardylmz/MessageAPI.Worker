using System;
using System.Collections.Generic;
using System.Text;

namespace MessageWorker.Application.Features.Shifts
{
    public record ShiftCommand(long telegramUserId, DateTime date);
}
