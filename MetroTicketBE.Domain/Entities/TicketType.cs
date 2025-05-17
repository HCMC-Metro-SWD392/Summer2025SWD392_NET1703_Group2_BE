namespace Domain.Entities;

public class TicketType
{
    public Guid Id { get; set; }
    public required string TypeName { get; set; }
    public required string Description { get; set; }
    
    public ICollection<SubscriptionTicket> SubscriptionTickets { get; set; } = new List<SubscriptionTicket>();
}