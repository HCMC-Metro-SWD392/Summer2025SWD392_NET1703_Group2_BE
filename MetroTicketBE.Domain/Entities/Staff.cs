using MetroTicket.Domain.Entities;

namespace MetroTicketBE.Domain.Entities;

public class Staff
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = null!;
    
    public User User { get; set; } = null!;
    public ICollection<StaffSchedule> StaffSchedules { get; set; } = null!;
}