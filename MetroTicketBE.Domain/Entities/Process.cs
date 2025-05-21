using MetroTicket.Domain.Entities;

namespace MetroTicketBE.Domain.Entities;

public class Process : BaseEntity<Guid, string, string>
{
    public Guid TicketId { get; set; }
    public Guid StationCheckInId { get; set; }
    public Guid StationCheckOutId { get; set; }
    public DateTime CheckInTime { get; set; }
    public DateTime CheckOutTime { get; set; }
    public Guid StatusId { get; set; }
    
    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    public Station StationCheckIn { get; set; } = null!;
    public Station StationCheckOut { get; set; } = null!;
    public Status Status { get; set; } = null!;

}