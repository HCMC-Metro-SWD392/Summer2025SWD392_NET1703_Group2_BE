using MetroTicketBE.Domain.DTO.Customer;
using MetroTicketBE.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroTicketBE.Domain.DTO.Auth
{
    public class GetAdminMangerDTO
    {
        public Guid Id { get; set; }
        public String? FullName { get; set; } = null!;
        public String? Address { get; set; } = null;
        public String? Sex { get; set; } = null;
        public DateOnly? DateOfBirth { get; set; }
        public String? UserName { get; set; } = null;
        public String? Email { get; set; } = null;
        public String? PhoneNumber { get; set; } = null;
        public String? IdentityId { get; set; } = null;
    }
}
