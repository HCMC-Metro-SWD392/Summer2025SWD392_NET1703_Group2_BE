using MetroTicket.Domain.Entities;

namespace MetroTicketBE.Domain.Entities;

public class Transaction
{
    public Guid Id { get; set; }    
    public required string UserId { get; set; }
    public double TotalPrice { get; set; }
    public Guid? PromotionId { get; set; }
    public Guid PaymentMethodId { get; set; }
    public Guid Status { get; set; }
    
    public ApplicationUser User { get; set; } = null!;

    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

    public Promotion Promotion { get; set; } = null!;
}