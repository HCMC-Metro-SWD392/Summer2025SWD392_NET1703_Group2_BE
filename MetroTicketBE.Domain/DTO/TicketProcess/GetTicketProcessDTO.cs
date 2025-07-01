using MetroTicketBE.Domain.Enums;

namespace MetroTicketBE.Domain.DTO.TicketProcess
{
    public class GetTicketProcessDTO
    {
        public string StationName { get; set; } = null!;
        public DateTime ProcessedAt { get; set; }
        public TicketProcessStatus Status { get; set; }
    }
}
