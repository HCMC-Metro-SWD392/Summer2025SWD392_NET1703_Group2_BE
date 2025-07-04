using MetroTicketBE.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroTicketBE.Domain.DTO.Ticket
{
    public class GetTicketDTO
    {
        public Guid Id { get; set; }
        public Guid? TicketRouteId { get; set; }
        public Guid? SubscriptionTicketId { get; set; }
        public string? FromStationRoute { get; set; } = null!;
        public string? ToStationRoute { get; set; } = null!;
        public string? FromStationSub { get; set; } = null!;
        public string? ToStationSub { get; set; } = null!;
        public int Price { get; set; }
        public string? TicketSerial { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public TicketStatus? TicketRtStatus { get; set; }
    }
}
