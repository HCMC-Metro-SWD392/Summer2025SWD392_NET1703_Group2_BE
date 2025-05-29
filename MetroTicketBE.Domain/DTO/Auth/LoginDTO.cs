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
        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        public required string PhoneNumber { get; set; }
        
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        public required string Password { get; set; }
    }
}
