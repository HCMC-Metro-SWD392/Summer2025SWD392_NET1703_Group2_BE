using MetroTicket.Domain.Entities;

namespace MetroTicketBE.Domain.Entities;

public class Transaction : BaseEntity
{
    public Guid Id { get; set; }
    public required string UserId { get; set; }
    public double TotalPrice { get; set; }
    public Guid? DiscountId { get; set; }
    public Guid PaymentMethodId { get; set; }
    public Guid StatusId { get; set; }
    
    public User User { get; set; } = null!;
    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    public ICollection<Discount> Discounts { get; set; } = new List<Discount>();
    public Status Status { get; set; } = null!;
}