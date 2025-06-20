namespace MetroTicketBE.Domain.DTO.SubscriptionTicket;

public class UpdateSubscriptionDTO
{
    public string? TicketName { get; set; } = null!;
    public int? Price { get; set; }
}