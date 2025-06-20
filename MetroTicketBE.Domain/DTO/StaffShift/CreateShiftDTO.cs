namespace MetroTicketBE.Domain.DTO.StaffShift;

public class CreateShiftDTO
{
    public string ShiftName { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
}