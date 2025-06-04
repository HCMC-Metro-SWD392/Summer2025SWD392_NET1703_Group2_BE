using MetroTicketBE.Domain.Enums;

namespace MetroTicketBE.Domain.Entities
{
    public class TicketRoute
    {
        public Guid Id { get; set; }
        public string TicketName { get; set; } = null!;
        public Guid StartStationId { get; set; }
        public Guid EndStationId { get; set; }
        public double? Distance { get; set; }
        public int Price { get; set; }
        public TimeSpan Expiration { get; set; }
        public TicketRoutStatus? Status { get; set; } = TicketRoutStatus.Active;

        public virtual Station StartStation { get; set; } = null!;
        public virtual Station EndStation { get; set; } = null!;
        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
}
