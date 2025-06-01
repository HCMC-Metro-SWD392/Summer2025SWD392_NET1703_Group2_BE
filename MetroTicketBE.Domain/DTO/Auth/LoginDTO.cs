using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroTicketBE.Domain.DTO.Auth
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "Vui lòng nhập email")]
        public required string Email { get; set; }
        
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        public required string Password { get; set; }
        public bool RememberMe { get; set; } = false;
    }
}
