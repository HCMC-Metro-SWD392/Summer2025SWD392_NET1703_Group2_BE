using MetroTicketBE.Domain.Enum;
using MetroTicketBE.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroTicketBE.Domain.DTO.TrainSchedule
{
    public class CreateTrainScheduleDTO
    {
        public Guid MetroLineId { get; set; }
        public int TravelTimeBetweenStationsInSeconds { get; set; }
        public int DwellTimeAtStationInSeconds { get; set; }
        public TimeSpan PeakHourMorningStart { get; set; }
        public TimeSpan PeakHourMorningEnd { get; set; }
        public TimeSpan PeakHourEveningStart { get; set; }
        public TimeSpan PeakHourEveningEnd { get; set; }
        public int PeakHourHeadwayInSeconds { get; set; }
        public int OffPeakHourHeadwayInSeconds { get; set; }
    }
}
