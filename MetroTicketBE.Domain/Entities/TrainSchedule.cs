using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroTicketBE.Domain.Entities
{
    public class TrainSchedule
    {
        public Guid Id { get; set; }
        public Guid StartStationId { get; set; }
        public TimeSpan StartTime { get; set; }

        public Station StartStation { get; set; } = null!;
    }
}
