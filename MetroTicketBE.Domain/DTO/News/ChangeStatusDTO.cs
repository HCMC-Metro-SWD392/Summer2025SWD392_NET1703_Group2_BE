using MetroTicketBE.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroTicketBE.Domain.DTO.News
{
    public class ChangeStatusDTO
    {
        public NewsStatus Status { get; set; }
        public string? RejectionReason { get; set; }
    }
}
