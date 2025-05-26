using MetroTicket.Domain.Entities;
using MetroTicketBE.Domain.Enum;

namespace MetroTicketBE.Domain.Entities;

public class StaffSchedule
{
    public Guid Id { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public StaffScheduleStatus Status { get; set; }
    
    public ICollection<Staff> Staffs { get; set; } = null!;
}