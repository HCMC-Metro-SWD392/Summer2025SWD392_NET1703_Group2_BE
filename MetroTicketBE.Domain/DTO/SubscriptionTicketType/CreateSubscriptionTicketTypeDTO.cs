namespace MetroTicketBE.Domain.DTO.SubscriptionTicketType;

public class CreateSubscriptionTicketTypeDTO
{
    public string Name { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public int Expiration { get; set; } // Thời gian hết hạn tính bằng ngày
    public int FareCoefficient { get; set; } = 1; // Hệ số giá vé, mặc định là 1.0
}