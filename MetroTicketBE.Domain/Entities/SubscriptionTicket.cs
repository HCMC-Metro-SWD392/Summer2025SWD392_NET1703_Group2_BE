using MetroTicket.Domain.Entities;

namespace MetroTicketBE.Domain.Entities;

public class SubscriptionTicket : BaseEntity
{
    public Guid Id { get; set; }
    public string TicketName { get; set; } = null!;
    public Guid TicketTypeId { get; set; }
    public double Price { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    
    public TicketType TicketType { get; set; } = null!;
    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}