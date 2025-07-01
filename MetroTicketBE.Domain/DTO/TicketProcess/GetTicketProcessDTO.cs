using MetroTicketBE.Domain.Enums;

namespace MetroTicketBE.Domain.DTO.TicketProcess
{
    public class GetTicketProcessDTO
    {
        public Guid Id { get; set; }
        public Guid TicketId { get; set; }
        public Guid StationId { get; set; }
        public DateTime ProcessedAt { get; set; }
        public TicketProcessStatus Status { get; set; }

    }
}
