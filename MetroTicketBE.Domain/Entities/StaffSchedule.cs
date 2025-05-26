using MetroTicket.Domain.Entities;

namespace MetroTicketBE.Domain.Entities;

public class StaffSchedule: BaseEntity<Guid, string, string>
{
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public Guid StatusId { get; set; }
    
    public Status Status { get; set; } = null!;
    public ICollection<Staff> Staffs { get; set; } = null!;
}