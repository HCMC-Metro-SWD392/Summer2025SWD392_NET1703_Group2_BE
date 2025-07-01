
using MetroTicketBE.Domain.Enums;

namespace MetroTicketBE.Domain.Entities;

public class Ticket
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public Guid? SubscriptionTicketId { get; set; }
    public Guid? TicketRouteId { get; set; }
    public int Price { get; set; }
    public string? TicketSerial { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public TicketStatus? TicketRtStatus { get; set; } = TicketStatus.Inactive;
    public string? QrCode { get; set; } = null!;

    public SubscriptionTicket? SubscriptionTicket { get; set; } = null!;
    public TicketRoute? TicketRoute { get; set; } = null!;
    public ICollection<PaymentTransaction> Transaction { get; set; } = new List<PaymentTransaction>();
    public ICollection<TicketProcess> TicketProcesses { get; set; } = new List<TicketProcess>();
}