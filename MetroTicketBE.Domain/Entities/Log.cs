using MetroTicketBE.Domain.Enum;

namespace MetroTicket.Domain.Entities
{
    public class Log
    {
        public Guid Id { get; set; }
        public string? Description { get; set; }
        public string UserId { get; set; }
        public LogType LogType { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public ApplicationUser User { get; set; } = null!;
    }
}
