using MetroTicket.Domain.Entities;
using MetroTicketBE.Domain.Enum;

namespace MetroTicketBE.Domain.Entities;

public class StaffSchedule
{
    public Guid Id { get; set; }
    public Guid ShiftId { get; set; }
    public Guid StaffId { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public DateOnly WorkingDate { get; set; }
    public Guid WorkingStationId { get; set; }
    public StaffScheduleStatus Status { get; set; }
    
    public Staff Staff { get; set; } = null!;
    public StaffShift Shift { get; set; } = null!;
    public Station WorkingStation { get; set; } = null!;
}