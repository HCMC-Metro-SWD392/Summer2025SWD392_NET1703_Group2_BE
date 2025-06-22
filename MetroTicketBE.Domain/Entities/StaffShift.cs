namespace MetroTicketBE.Domain.Entities;

public class StaffShift
{
    public Guid Id { get; set; }
    public string ShiftName { get; set; } = string.Empty!;
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    
    public ICollection<StaffSchedule> StaffSchedules { get; set; } = new List<StaffSchedule>();
}