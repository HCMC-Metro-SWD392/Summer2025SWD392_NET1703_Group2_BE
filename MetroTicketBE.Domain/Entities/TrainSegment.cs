using MetroTicket.Domain.Entities;

namespace MetroTicketBE.Domain.Entities;

public class TrainSegment : BaseEntity
{
    public Guid Id { get; set; }
    public Guid TrainId { get; set; }
    public Guid StationStartId { get; set; }
    public Guid StationEndId { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    
    public Station StationStart { get; set; } = null!;
    public Station StationEnd { get; set; } = null!;
    public Train Train { get; set; } = null!;
}