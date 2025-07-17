using MetroTicketBE.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroTicketBE.Domain.DTO.News
{
    public class UpdateNewsDTO
    {
        public string? Title { get; set; } = null!;
        public string? Content { get; set; } = null!;
        public string? Summary { get; set; }
        public string? ImageUrl { get; set; }
        public string? Category { get; set; }
    }
}
