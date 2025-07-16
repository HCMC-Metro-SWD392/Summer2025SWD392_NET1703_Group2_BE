using MetroTicketBE.Domain.Enum;

namespace MetroTicketBE.Domain.DTO.StaffSchedule;

public class GetScheduleDTO
{
    public Guid Id { get; set; }
    public Guid StaffId { get; set; }
    public string StaffFullName { get; set; }
    public Guid ShiftId { get; set; }
    public string ShiftName { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public DateOnly WorkingDate { get; set; }
    public Guid StationId { get; set; }
    public string StationName { get; set; }
    public string Status { get; set; }
}
