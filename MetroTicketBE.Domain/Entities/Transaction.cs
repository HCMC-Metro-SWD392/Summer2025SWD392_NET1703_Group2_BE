using MetroTicket.Domain.Entities;

namespace MetroTicketBE.Domain.Entities;

public class Transaction : BaseEntity<Guid, string, string>
{
    public required string UserId { get; set; }
    public double TotalPrice { get; set; }
    public Guid? PromotionId { get; set; }
    public Guid PaymentMethodId { get; set; }
    public Guid StatusId { get; set; }
    
    public User User { get; set; } = null!;
    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    public ICollection<Promotion> Promotions { get; set; } = new List<Promotion>();
    public Status Status { get; set; } = null!;
}