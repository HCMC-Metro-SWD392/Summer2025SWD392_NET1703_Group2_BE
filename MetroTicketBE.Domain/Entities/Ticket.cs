
using MetroTicket.Domain.Entities;

namespace MetroTicketBE.Domain.Entities;

public class Ticket : BaseEntity
{
    public Guid Id { get; set; }
    public Guid? SubscriptionTicketId { get; set; }
    public Guid RouteId { get; set; }
    public Guid TransactionId { get; set; }
    public long TicketNumber { get; set; }
    public string Description { get; set; } = null!;
    public TimeSpan Expiration { get; set; }
    public string QrCode { get; set; } = null!;

    public SubscriptionTicket? SubscriptionTicket { get; set; }
    public TicketRoute Route { get; set; } = null!;
    public Transaction Transaction { get; set; } = null!;
}