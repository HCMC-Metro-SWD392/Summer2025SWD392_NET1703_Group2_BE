using MetroTicketBE.Domain.Enum;
using System.ComponentModel.DataAnnotations.Schema;

namespace MetroTicketBE.Domain.Entities;

public class PaymentTransaction
{
    public Guid Id { get; set; }    
    
    public string? OrderCode { get; set; }
    public string? DataJson { get; set; }
    public Guid? TicketId { get; set; }
    public Guid CustomerId { get; set; }
    public double TotalPrice { get; set; }
    public Guid? PromotionId { get; set; }
    public Guid PaymentMethodId { get; set; }
    public PaymentStatus Status { get; set; }
    
    public Ticket Ticket { get; set; } = null!;

    public Customer Customer { get; set; } = null!;

    public Ticket Ticket_ { get; set; } = null!;

    public Promotion Promotion { get; set; } = null!;

    public ICollection<PaymentMethod> PaymentMethods { get; set; } = new List<PaymentMethod>();
}