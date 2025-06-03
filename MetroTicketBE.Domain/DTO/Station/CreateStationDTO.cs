using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroTicketBE.Domain.DTO.Station
{
    public class CreateStationDTO
    {
        public required string Name { get; set; }
        public string Address { get; set; } = null!;
        public string Description { get; set; } = null!;
    }

    public class UpdateStationDTO 
    {
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? Description { get; set; }
    }
}
