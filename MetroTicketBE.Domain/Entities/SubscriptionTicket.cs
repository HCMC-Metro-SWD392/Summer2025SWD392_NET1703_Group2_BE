using MetroTicket.Domain.Entities;
using MetroTicketBE.Domain.Enum;
using MetroTicketBE.Domain.Enums;

namespace MetroTicketBE.Domain.Entities;

public class SubscriptionTicket
{
    public Guid Id { get; set; }
    public string TicketName { get; set; } = null!;
    public SubscriptionTicketType TicketType { get; set; }
    public int Price { get; set; }
    public TimeSpan Expiration { get; set; }
    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}