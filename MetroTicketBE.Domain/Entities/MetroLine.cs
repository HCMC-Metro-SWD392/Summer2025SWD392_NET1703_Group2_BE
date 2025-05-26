using MetroTicket.Domain.Entities;

namespace MetroTicketBE.Domain.Entities
{
    public class MetroLine
    {
        public Guid Id { get; set; }
        public Guid FareRuleId { get; set; }
        public required int MetroLineNumber { get; set; }
        public string? MetroName { get; set; }
        public Guid StartStationId { get; set; }
        public Guid EndStationId { get; set; }

        public Station StartStation { get; set; } = null!;
        public Station EndStation { get; set; } = null!;

        public ICollection<MetroLineStation> MetroLineStations { get; set; } = new List<MetroLineStation>();

        public FareRule FareRule { get; set; } = null!;
    }

}
