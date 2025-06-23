using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroTicketBE.Domain.DTO.Staff
{
    public class GetStaffDTO
    {
        public Guid Id { get; set; }
        public string UserId { get; set; } = null!;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? IdentityId { get; set; }
        public string Sex { get; set; } = string.Empty;
        public DateOnly? DateOfBirth { get; set; }
    }
}
