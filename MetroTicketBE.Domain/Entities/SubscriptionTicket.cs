using MetroTicket.Domain.Entities;
using MetroTicketBE.Domain.Enum;

namespace MetroTicketBE.Domain.Entities;

public class SubscriptionTicket
{
    public Guid Id { get; set; }
    public string TicketName { get; set; } = null!;
    public Guid TicketTypeId { get; set; }
    public int Expiration { get; set; } // Thời gian hết hạn tính bằng ngày
    public int Price { get; set; }
    public Guid StartStationId { get; set; }
    public Guid EndStationId { get; set; }
    public double Distance { get; set; }
    
    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    public SubscriptionTicketType TicketType { get; set; }

    public Station StartStation { get; set; } = null!;
    public Station EndStation { get; set; } = null!;
}