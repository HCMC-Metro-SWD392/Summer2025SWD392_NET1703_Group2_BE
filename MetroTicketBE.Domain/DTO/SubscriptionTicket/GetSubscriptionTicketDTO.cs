using MetroTicketBE.Domain.Enum;

namespace MetroTicketBE.Domain.DTO.SubscriptionTicket
{
    public class GetSubscriptionTicketDTO
    {
        public Guid Id { get; set; }
        public string TicketName { get; set; } = null!;
        public required TicketType TicketType { get; set; }
        public int Price { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
