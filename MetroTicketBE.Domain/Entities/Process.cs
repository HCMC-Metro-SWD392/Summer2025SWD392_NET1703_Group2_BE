namespace MetroTicketBE.Domain.Entities;

public class Process
{
    public Guid Id { get; set; }
    public Guid TicketId { get; set; }
    public Guid StationCheckInId { get; set; }
    public Guid StationCheckOutId { get; set; }
    public DateTime CheckInTime { get; set; }
    public DateTime CheckOutTime { get; set; }
    public Guid StatusId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    public Ticket Ticket { get; set; } = null!;
    public ICollection<Station> StationCheckIn { get; set; } = null!;
    public ICollection<Station> StationCheckOut { get; set; } = null!;
    
}