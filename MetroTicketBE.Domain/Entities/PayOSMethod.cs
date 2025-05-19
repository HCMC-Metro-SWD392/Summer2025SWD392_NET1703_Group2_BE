namespace MetroTicketBE.Domain.Entities;

public class PayOSMethod
{
    public Guid Id { get; set; }
    public Guid PaymentMethodId { get; set; }
    public long TransactionNumber { get; set; }
    public double TotalPrice { get; set; }
    public string Description { get; set; } = null!;
    public string CancelUrl { get; set; } = null!;
    public string ReturnUrl { get; set; } = null!;
    public DateTime ExpiredAt { get; set; }
    public string Signature { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid StatusId { get; set; }
    public string? CancelReason { get; set; } = null!;
    
    public Status Status { get; set; } = null!;
    public PaymentMethod PaymentMethod { get; set; } = null!;
}