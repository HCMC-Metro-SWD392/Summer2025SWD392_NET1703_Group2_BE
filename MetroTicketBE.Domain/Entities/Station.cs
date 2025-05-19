namespace MetroTicketBE.Domain.Entities;

public class Station
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string Address { get; set; } = null!;
    public string Description { get; set; } = null!;

    public ICollection<TimeLine> TimeLines { get; set; } = new List<TimeLine>();
    public ICollection<TicketRoute> TicketRoutes { get; set; } = new List<TicketRoute>();
}