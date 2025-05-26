using MetroTicket.Domain.Entities;
using MetroTicketBE.Domain.Enum;

namespace MetroTicketBE.Domain.Entities;

public class SubscriptionTicket
{
    public Guid Id { get; set; }
    public string TicketName { get; set; } = null!;
    public required TicketType TicketType { get; set; }
    public double Price { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}