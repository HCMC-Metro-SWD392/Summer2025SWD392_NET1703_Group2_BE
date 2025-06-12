using MetroTicketBE.Domain.Enums;

namespace MetroTicketBE.Domain.DTO.SubscriptionTicket;

public class CreateSubscriptionDTO
{
    public string TicketName { get; set; } = null!;
    public SubscriptionTicketType TicketType { get; set; }
    public int Price { get; set; }
    public Guid StartStationId { get; set; }
    public Guid EndStationId { get; set; }
}