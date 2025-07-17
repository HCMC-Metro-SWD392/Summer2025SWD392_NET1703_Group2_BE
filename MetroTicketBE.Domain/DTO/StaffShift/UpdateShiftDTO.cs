namespace MetroTicketBE.Domain.DTO.StaffShift;

public class UpdateShiftDTO
{
    public string? ShiftName { get; set; }
    public TimeSpan? StartTime { get; set; }
    public TimeSpan? EndTime { get; set; }
}