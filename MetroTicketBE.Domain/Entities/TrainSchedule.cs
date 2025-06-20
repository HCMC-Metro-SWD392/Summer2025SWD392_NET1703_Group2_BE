using MetroTicketBE.Domain.Enum;
using MetroTicketBE.Domain.Enums;

namespace MetroTicketBE.Domain.Entities
{
    public class TrainSchedule
    {
        public Guid Id { get; set; }
        public Guid? TrainId { get; set; }
        public Guid MetroLineId { get; set; }
        public Guid StationId { get; set; }
        public TimeSpan StartTime { get; set; }
        public TrainScheduleType Direction { get; set; }
        public TrainScheduleStatus Status { get; set; }

        public Station Station { get; set; } = null!;

        public MetroLine MetroLine { get; set; } = null!;

        public ICollection<Train> Trains { get; set; } = new List<Train>();
    }
}
