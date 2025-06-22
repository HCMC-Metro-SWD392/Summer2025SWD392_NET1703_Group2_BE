using MetroTicketBE.Domain.Enum;

namespace MetroTicketBE.Domain.DTO.StaffSchedule;

public class GetScheduleDTO
{
    public Guid Id { get; set; }
    public StaffInfoDto Staff { get; set; }
    public ShiftInfoDto Shift { get; set; }
    public DateOnly WorkingDate { get; set; }
    public StationInfoDto WorkingStation { get; set; }
    public StaffScheduleStatus Status { get; set; }
}
public class StaffInfoDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
}

public class ShiftInfoDto
{
    public Guid Id { get; set; }
    public string ShiftName { get; set; } = string.Empty;
}

public class StationInfoDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}