
using MetroTicket.Domain.Entities;

namespace MetroTicketBE.Domain.Entities;

public class Ticket
{
    public Guid Id { get; set; }
    public Guid? SubscriptionTicketId { get; set; }
    public Guid TicketRouteId { get; set; }
    public Guid TransactionId { get; set; }
    public required string TicketSerial { get; set; }
    public string Description { get; set; } = null!;
    public TimeSpan Expiration { get; set; }
    public string QrCode { get; set; } = null!;

    public SubscriptionTicket? SubscriptionTicket { get; set; }
    public TicketRoute TicketRoute { get; set; } = null!;
    public Transaction Transaction { get; set; } = null!;
}