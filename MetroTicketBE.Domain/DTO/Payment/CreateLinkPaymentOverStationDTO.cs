using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroTicketBE.Domain.DTO.Payment
{
    public class CreateLinkPaymentOverStationDTO
    {
        public Guid TicketId { get; set; }
        public Guid StationId { get; set; }
    }
}
