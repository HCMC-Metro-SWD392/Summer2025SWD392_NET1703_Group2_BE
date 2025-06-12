namespace MetroTicketBE.Domain.Entities;

public class SubscriptionTicketType
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public int Expiration { get; set; } // Thời gian hết hạn tính bằng ngày
    public int FareCoefficient { get; set; } = 1; // Hệ số giá vé, mặc định là 1.0
    public ICollection<SubscriptionTicket> SubscriptionTickets { get; set; } = new List<SubscriptionTicket>();
}