using MetroTicketBE.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroTicketBE.Domain.Entities
{
    public class TicketProcess
    {
        public Guid Id { get; set; }
        public Guid TicketId { get; set; }
        public Guid StationId { get; set; }
        public DateTime ProcessedAt { get; set; }
        public TicketProcessStatus Status { get; set; }

        public Ticket Ticket { get; set; } = null!;
        public Station Station { get; set; } = null!;
    }
}
