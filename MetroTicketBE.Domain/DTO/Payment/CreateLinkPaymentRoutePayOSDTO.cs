using MetroTicketBE.Domain.DTO.SubscriptionTicket;
using MetroTicketBE.Domain.DTO.TicketRoute;
using MetroTicketBE.Domain.Enum;

namespace MetroTicketBE.Domain.DTO.Payment
{
    public class CreateLinkPaymentRoutePayOSDTO
    {
        public List<GetTicketRouteDTO>? TicketRoute { get; set; } = new();
        public List<GetSubscriptionTicketDTO>? SubscriptionTickets { get; set; } = new();
        public string? CodePromotion { get; set; }
        public string? Description { get; set; }
        public string CancelUrl { get; set; } = null!;
        public string ReturnUrl { get; set; } = null!;
        public DateTime ExpiredAt { get; set; }
        public string Signature { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public PaymentStatus Status { get; set; }
        public string? CancelReason { get; set; } = null!;
    }
}
