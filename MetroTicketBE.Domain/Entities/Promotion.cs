using System.ComponentModel.DataAnnotations;

namespace MetroTicketBE.Domain.Entities;

public class Promotion
{
    public Guid Id { get; set; }
    public string Code { get; set; } = null!;
    [Range(0, 100)]
    public decimal Percentage { get; set; }
    public string Description { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpirationDate { get; set; }

    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}