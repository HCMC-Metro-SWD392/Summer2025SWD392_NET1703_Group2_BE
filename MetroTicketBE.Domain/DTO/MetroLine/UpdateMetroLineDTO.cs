namespace MetroTicketBE.Domain.DTO.MetroLine;

public class UpdateMetroLineDTO
{
    public string? MetroLineNumber { get; set; }
    public string? MetroName { get; set; }
    public Guid? StartStationId { get; set; }
    public Guid? EndStationId { get; set; }
    public TimeSpan? StartTime { get; set; }
    public TimeSpan? EndTime { get; set; }
}