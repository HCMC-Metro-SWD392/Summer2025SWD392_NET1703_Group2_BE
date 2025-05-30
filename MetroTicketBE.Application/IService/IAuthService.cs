using MetroTicketBE.Domain.DTO.Auth;

namespace MetroTicketBE.Application.IService
{
    public interface IAuthService
    {
        Task<ResponseDTO> RegisterCustomer(RegisterCustomerDTO registerCustomerDTO);
        Task<ResponseDTO> LoginUser(LoginDTO loginDTO);
        Task<ResponseDTO> SendVerifyEmail(string email, string confirmationLink);
    }
}
