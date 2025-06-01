using MetroTicketBE.Domain.Entities;
using MetroTicketBE.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroTicketBE.Domain.DTO.Payos
{
    public class CreateLinkPaymentRouteDTO
    {
        public long OderCode { get; set; }
        public List<TicketRoute> Ticket { get; set; } = new List<TicketRoute>();
        public string CodePromotion { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string CancelUrl { get; set; } = null!;
        public string ReturnUrl { get; set; } = null!;
        public DateTime ExpiredAt { get; set; }
        public string Signature { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public PaymentStatus Status { get; set; }
        public string? CancelReason { get; set; } = null!;
    }
}
