using MetroTicket.Domain.Entities;

namespace MetroTicketBE.Domain.Entities
{
    public class MetroLine
    {
        public Guid Id { get; set; }
        public required string MetroLineNumber { get; set; }
        public string? MetroName { get; set; }
        public Guid StartStationId { get; set; }
        public Guid EndStationId { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public Station StartStation { get; set; } = null!;
        public Station EndStation { get; set; } = null!;
        public bool IsActive { get; set; } = true;

        public ICollection<MetroLineStation> MetroLineStations { get; set; } = new List<MetroLineStation>();

        public ICollection<TrainSchedule> TrainSchedules { get; set; } = new List<TrainSchedule>();

    }

}
