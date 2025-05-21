using MetroTicket.Domain.Entities;

namespace MetroTicketBE.Domain.Entities;

public class TicketRoute : BaseEntity<Guid, string, string>
{
    public Guid FirstStationId { get; set; }
    public Guid LastStationId { get; set; }
    public double Price { get; set; }
    
    public Station FirstStation { get; set; } = null!;
    public Station LastStation { get; set; } = null!;
    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}