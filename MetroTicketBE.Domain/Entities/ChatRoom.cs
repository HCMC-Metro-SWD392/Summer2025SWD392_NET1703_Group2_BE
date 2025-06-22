using MetroTicketBE.Domain.Constants;

namespace MetroTicketBE.Domain.Entities;

public class ChatRoom
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; }
    public string CreatorUserId { get; set; }
    public string? PartnerUserId { get; set; } // Người tham gia, có thể null
    public StaticRoomStatus Status { get; set; } = StaticRoomStatus.Open;
}