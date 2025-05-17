namespace Domain.Entities;

public class Transaction
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public double TotalPrice { get; set; }
    public Guid? DiscountId { get; set; }
    public Guid PaymentMethodId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}