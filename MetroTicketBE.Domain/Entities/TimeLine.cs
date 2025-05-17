namespace MetroTicketBE.Domain.Entities;

public class TimeLine
{
    public Guid Id { get; set; }
    public Guid StationStartId { get; set; }
    public Guid StationEndId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    
    public ICollection<Station> StationStart { get; set; } = null!;
    public ICollection<Station> StationEnd { get; set; } = null!;
}