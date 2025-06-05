
namespace MetroTicketBE.Domain.Entities;

public class Ticket
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public Guid? SubscriptionTicketId { get; set; }
    public Guid? TicketRouteId { get; set; }
    public Guid TransactionId { get; set; }
    public required string TicketSerial { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string QrCode { get; set; } = null!;

    public SubscriptionTicket? SubscriptionTicket { get; set; } = null!;
    public TicketRoute? TicketRoute { get; set; } = null!;
    public PaymentTransaction Transaction { get; set; } = null!;
}