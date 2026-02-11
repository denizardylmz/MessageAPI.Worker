using System;
using System.Collections.Generic;
using System.Text;

namespace MessageWorker.Infrastructure.Persistence.Entities
{
    public class ShiftEntity
    {
        public Guid Id { get; set; }
        public long UserId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

}
