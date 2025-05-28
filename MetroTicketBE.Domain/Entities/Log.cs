using MetroTicketBE.Domain.Enum;

namespace MetroTicket.Domain.Entities
{
    public class Log
    {
        public Guid Id { get; set; }
        public Guid LogTypeId { get; set; }
        public string? Description { get; set; }
        public LogType LogType { get; set; }
        public User User { get; set; } = null!;
    }
}
