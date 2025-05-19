namespace MetroTicket.Domain.Entities
{
    public class LogType
    {
        public Guid Id { get; set; }
        public string? TypeName { get; set; }
        public ICollection<Log> Logs { get; set; } = null!;
    }
}
