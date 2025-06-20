using MetroTicketBE.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroTicketBE.Domain.Entities
{
    public class FormAttachment
    {
        public Guid Id { get; set; }
        public required string Url { get; set; }
        public string FileName { get; set; } = null!;

        public Guid FormRequestId { get; set; }
        public FormRequest FormRequest { get; set; } = null!;
    }
}
