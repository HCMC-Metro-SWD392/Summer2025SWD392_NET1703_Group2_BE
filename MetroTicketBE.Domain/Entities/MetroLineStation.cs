using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroTicketBE.Domain.Entities
{
    public class MetroLineStation
    {
        public Guid Id { get; set; }
        public Guid MetroLineId { get; set; }
        public Guid StationId { get; set; }
        public double DistanceFromStart { get; set; }
        public int StationOder { get; set; }

        public MetroLine MetroLine { get; set; } = null!;
        public Station Station { get; set; } = null!;
    }
}
