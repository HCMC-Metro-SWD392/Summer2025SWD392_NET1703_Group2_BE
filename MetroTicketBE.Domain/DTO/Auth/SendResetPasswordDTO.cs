using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroTicketBE.Domain.DTO.Auth
{
    public class SendResetPasswordDTO
    {
        public string Email { get; set; } = null!;
    }
}
