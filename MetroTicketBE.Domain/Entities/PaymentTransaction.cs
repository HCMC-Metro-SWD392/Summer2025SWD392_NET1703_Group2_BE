using MetroTicketBE.Domain.Enum;
using System.ComponentModel.DataAnnotations.Schema;

namespace MetroTicketBE.Domain.Entities;

public class PaymentTransaction
{
    public Guid Id { get; set; }    
    
    public string? OrderCode { get; set; }
    public string? ItemDataJson { get; set; }
    public Guid CustomerId { get; set; }
    public double TotalPrice { get; set; }
    public Guid? PromotionId { get; set; }
    public Guid PaymentMethodId { get; set; }
    public PaymentStatus Status { get; set; }
    
    public Customer Customer { get; set; } = null!;

    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

    public Promotion Promotion { get; set; } = null!;

    public ICollection<PaymentMethod> PaymentMethods { get; set; } = new List<PaymentMethod>();
}