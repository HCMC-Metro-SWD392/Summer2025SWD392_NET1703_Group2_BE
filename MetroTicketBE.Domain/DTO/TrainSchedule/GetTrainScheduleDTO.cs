using MetroTicketBE.Domain.Enum;
using MetroTicketBE.Domain.Enums;

namespace MetroTicketBE.Domain.DTO.TrainSchedule
{
    public class GetTrainScheduleDTO
    {
        public Guid Id { get; set; }
        public Guid MetroLineId { get; set; }
        public string MetroLineName { get; set; } = null!;
        public Guid StationId { get; set; }
        public string StationName { get; set; } = null!;
        public TimeSpan StartTime { get; set; }
        public TrainScheduleType Direction { get; set; }
        public TrainScheduleStatus Status { get; set; }
    }
}
