using MetroTicketBE.Domain.DTO.SubscriptionTicket;
using MetroTicketBE.Domain.DTO.TicketRoute;
using MetroTicketBE.Domain.Enum;

namespace MetroTicketBE.Domain.DTO.Payment
{
    public class CreateLinkPaymentPayOSDTO
    {
        public Guid? TicketRouteId { get; set; }
        public Guid? SubscriptionTicketId { get; set; }
        public string? CodePromotion { get; set; }
    }
}
