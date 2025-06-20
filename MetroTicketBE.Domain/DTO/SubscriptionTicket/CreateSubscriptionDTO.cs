
using MetroTicketBE.Domain.Entities;

namespace MetroTicketBE.Domain.DTO.SubscriptionTicket;

public class CreateSubscriptionDTO
{
    public Guid TicketTypeId { get; set; }
    public Guid StartStationId { get; set; }
    public Guid EndStationId { get; set; }
}