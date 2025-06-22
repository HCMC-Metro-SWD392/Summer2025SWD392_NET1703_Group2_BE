using MetroTicketBE.Domain.DTO.Auth;
using System.Security.Claims;

namespace MetroTicketBE.Application.IService
{
    public interface IAuthService
    {
        Task<ResponseDTO> RegisterCustomer(RegisterCustomerDTO registerCustomerDTO);
        Task<ResponseDTO> LoginUser(LoginDTO loginDTO);
        Task<ResponseDTO> SendVerifyEmail(string email);
        Task<ResponseDTO> VerifyEmail(string userId, string token);
        Task<ResponseDTO> Logout(ClaimsPrincipal user);
        Task<ResponseDTO> SetStaffRole(string email);
        Task<ResponseDTO> CreateStaffAsync(RegisterCustomerDTO dto);
    }
}
