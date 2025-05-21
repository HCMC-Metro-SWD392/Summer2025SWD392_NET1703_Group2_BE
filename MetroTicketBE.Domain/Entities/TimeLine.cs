using MetroTicket.Domain.Entities;

namespace MetroTicketBE.Domain.Entities;

public class TimeLine: BaseEntity<Guid, string, string>
{
    public Guid TrainSegmentId { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    
    public TrainSegment TrainSegment { get; set; } = null!;
    public ICollection<Train> Trains { get; set; } = new List<Train>();
}