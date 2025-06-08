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

        public virtual Station StartStation { get; set; } = null!;
        public virtual Station EndStation { get; set; } = null!;
        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
}
