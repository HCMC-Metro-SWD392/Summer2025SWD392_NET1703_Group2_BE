using MetroTicketBE.Domain.Enum;
using MetroTicketBE.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroTicketBE.Domain.DTO.TrainSchedule
{
    public class UpdateTrainScheduleDTO
    {
        public Guid Id { get; set; }
        //public Guid? TrainId { get; set; }
        public Guid? MetroLineId { get; set; }
        public Guid? StationId { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TrainScheduleType? Direction { get; set; }
        public TrainScheduleStatus? Status { get; set; }
    }
}
