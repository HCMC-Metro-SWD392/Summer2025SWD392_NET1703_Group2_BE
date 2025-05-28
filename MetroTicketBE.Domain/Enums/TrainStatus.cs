using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroTicketBE.Domain.Enum
{
    public enum TrainStatus
    {
        Running = 0, //Train is running
        Stopped = 1, // Train is stopped at a station
        Maintenance = 2, // Train is under maintenance
        Delayed = 3, // Train is delayed
        Cancelled = 4 // Train service is cancelled
    }
}
