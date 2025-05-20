using MetroTicket.Domain.Entities;

namespace MetroTicketBE.Domain.Entities;

public class TicketRoute : BaseEntity
{
    public Guid Id { get; set; }
    public Guid FirstStationId { get; set; }
    public Guid LastStationId { get; set; }
    public double Price { get; set; }
    
    public Station FirstStation { get; set; } = null!;
    public Station LastStation { get; set; } = null!;
    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}