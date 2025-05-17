namespace MetroTicketBE.Domain.Entities;

public class Discount
{
    public Guid Id { get; set; }
    public string Code { get; set; } = null!;
    public decimal Percentage { get; set; }
    public string Description { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpirationDate { get; set; }
}