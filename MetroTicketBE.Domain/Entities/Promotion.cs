using MetroTicketBE.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace MetroTicketBE.Domain.Entities;

public class Promotion
{
    public Guid Id { get; set; }
    public string Code { get; set; } = null!;
    [Range(0, 100)]
    public decimal? Percentage { get; set; }
    public int? FixedAmount { get; set; }
    public PromotionType PromotionType { get; set; }
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<PaymentTransaction> Transactions { get; set; } = new List<PaymentTransaction>();
}