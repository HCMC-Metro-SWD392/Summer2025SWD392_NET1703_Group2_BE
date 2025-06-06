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
        public Guid CustomerId { get; set; }
        public Guid? SubscriptionTicketId { get; set; }
        public Guid? TicketRouteId { get; set; }
        public Guid TransactionId { get; set; }
        public required string TicketSerial { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string QrCode { get; set; } = null!;
    }
}
