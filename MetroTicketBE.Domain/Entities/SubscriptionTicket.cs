using MetroTicket.Domain.Entities;
using MetroTicketBE.Domain.Enum;

namespace MetroTicketBE.Domain.Entities;

public class SubscriptionTicket
{
    public Guid Id { get; set; }
    public string TicketName { get; set; } = null!;
    public int Price { get; set; }
    public TimeSpan Expiration { get; set; }
    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}