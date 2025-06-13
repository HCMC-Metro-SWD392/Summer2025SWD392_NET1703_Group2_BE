using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MetroTicketBE.Domain.Entities
{
    public class FareRule
    {
        public Guid Id { get; set; }
        public double MinDistance { get; set; }
        public double MaxDistance { get; set; }
        public int Fare { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}
