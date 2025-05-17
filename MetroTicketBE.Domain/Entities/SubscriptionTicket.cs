namespace MetroTicketBE.Domain.Entities;

public class SubscriptionTicket
{
    public Guid Id { get; set; }
    public Guid TicketTypeId { get; set; }
    public double Price { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    public TicketType TicketType { get; set; } = null!;
    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}