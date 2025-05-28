using MetroTicket.Domain.Entities;

namespace MetroTicketBE.Domain.Entities;

public class Process
{
    public Guid Id { get; set; }
    public Guid TicketId { get; set; }
    public Guid StationCheckInId { get; set; }
    public Guid StationCheckOutId { get; set; }
    public DateTime CheckInTime { get; set; }
    public DateTime CheckOutTime { get; set; }
    
    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    public Station StationCheckIn { get; set; } = null!;
    public Station StationCheckOut { get; set; } = null!;

}