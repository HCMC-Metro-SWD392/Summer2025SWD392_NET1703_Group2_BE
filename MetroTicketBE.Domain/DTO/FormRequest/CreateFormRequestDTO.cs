using MetroTicketBE.Domain.Enum;
using MetroTicketBE.Domain.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroTicketBE.Domain.DTO.FormRequest
{
    public class CreateFormRequestDTO
    {
        public string Title { get; set; } = null!;
        public string? Content { get; set; }
        public CustomerType? CustomerType { get; set; }
        public FormRequestType FormRequestType { get; set; }
        public List<string> AttachmentKeys { get; set; } = new();
    }
}
