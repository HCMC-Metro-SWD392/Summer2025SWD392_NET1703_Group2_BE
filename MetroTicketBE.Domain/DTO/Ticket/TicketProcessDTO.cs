using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroTicketBE.Domain.DTO.Ticket
{
    public class TicketProcessDTO
    {
        public string QrCode { get; set; } = null!;
        public Guid StationId { get; set; }
    }
}
