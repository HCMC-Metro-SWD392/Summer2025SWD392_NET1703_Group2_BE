using MetroTicketBE.Domain.Enums;

namespace MetroTicketBE.Domain.DTO.SubscriptionTicket;

public class CreateSubscriptionDTO
{
    public string TicketName { get; set; } = null!;
    public SubscriptionTicketType TicketType { get; set; }
    public int Price { get; set; }
    public TimeSpan Expiration { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

}