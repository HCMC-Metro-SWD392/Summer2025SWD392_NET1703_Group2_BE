namespace MetroTicket.Domain.Entities
{
    public abstract class BaseEntity<IID, CID, UID>
    {
        public IID Id { get; set; }
        public CID CreatedById { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public UID UpdatedById { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    }
}
