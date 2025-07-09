using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroTicketBE.Domain.DTO.Auth
{
    public class LoginByGoogleDTO
    {
        public required string Email { get; set; }

        public required string FullName { get; set; }

        public bool RememberMe { get; set; } = true;
    }
}
