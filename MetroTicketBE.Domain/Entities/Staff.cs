using MetroTicket.Domain.Entities;

namespace MetroTicketBE.Domain.Entities;

public class Staff
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = null!;
    public Guid StaffScheduleId { get; set; }
    
    public User User { get; set; } = null!;
    public StaffSchedule StaffSchedule { get; set; } = null!;
}