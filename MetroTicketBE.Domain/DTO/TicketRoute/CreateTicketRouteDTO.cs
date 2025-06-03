using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroTicketBE.Domain.DTO.TicketRoute
{
    public class CreateTicketRouteDTO
    {
        public Guid StartStationId { get; set; }
        public Guid EndStationId { get; set; }
    }
}
