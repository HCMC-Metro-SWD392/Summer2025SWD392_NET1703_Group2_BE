namespace Domain.Entities;

public class TicketRoute
{
    public Guid Id { get; set; }
    public Guid FirstStationId { get; set; }
    public Guid LastStationId { get; set; }
    public double Price { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    public Station FirstStation { get; set; } = null!;
    public Station LastStation { get; set; } = null!;
}