
using System.ComponentModel.DataAnnotations;

namespace MetroTicketBE.Domain.DTO.Auth
{
    public class RegisterCustomerByGoogleDTO
    {

        public required string Email { get; set; } = null!;

        public required string FullName { get; set; }


    }
}
