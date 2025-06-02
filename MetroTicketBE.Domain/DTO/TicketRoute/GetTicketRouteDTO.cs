using MetroTicketBE.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroTicketBE.Domain.DTO.TicketRoute
{
    public class GetTicketRouteDTO
    {
        public Guid Id { get; set; }
        public string TicketName { get; set; } = null!;
        public Guid StartStationId { get; set; }
        public Guid EndStationId { get; set; }
        public double? Distance { get; set; }
        public int Price { get; set; }
        public TicketRoutStatus? Status { get; set; }
    }
}
