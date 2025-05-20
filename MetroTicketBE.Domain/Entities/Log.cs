namespace MetroTicket.Domain.Entities
{
    public class Log : BaseEntity<Guid, string, string>
    {
        public Guid LogTypeId { get; set; }
        public string? Description { get; set; }
        public LogType LogType { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}
