using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroTicketBE.Domain.DTO.MetroLineStation
{
    public class CreateMetroLineStationDTO
    {
        public Guid MetroLineId { get; set; }
        public Guid StationId { get; set; }
        public double DistanceFromStart { get; set; }
        public int StationOder { get; set; }
    }
}
