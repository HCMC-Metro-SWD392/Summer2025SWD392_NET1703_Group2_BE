
namespace Domain.Entities;

public class Ticket
{
    public Guid Id { get; set; }
    public Guid? SubscriptionTicketId { get; set; }
    public Guid RouteId { get; set; }
    public Guid TransactionId { get; set; }
    public long TicketNumber { get; set; }
    public string Description { get; set; } = null!;
    public double Expiration { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public string QrCode { get; set; } = null!;
    
    public SubscriptionTicket? SubscriptionTicket { get; set; }
    public Transaction Transaction { get; set; } = null!;
}