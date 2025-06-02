using System;
using System.Collections.Generic;
using System.Linq;
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

    }
}
