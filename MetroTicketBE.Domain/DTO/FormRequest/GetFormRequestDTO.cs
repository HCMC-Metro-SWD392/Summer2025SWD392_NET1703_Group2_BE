using MetroTicketBE.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroTicketBE.Domain.DTO.FormRequest
{
    public class GetFormRequestDTO
    {
        public Guid Id { get; set; }
        public required string SenderId { get; set; }
        public string Title { get; set; } = null!;
        public string? Content { get; set; }
        public required FormRequestType FormRequestType { get; set; }
        public string? ReviewerId { get; set; }
        public string? RejectionReason { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public FormStatus Status { get; set; }
    }
}
