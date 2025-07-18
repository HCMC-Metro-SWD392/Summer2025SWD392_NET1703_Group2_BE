using MetroTicketBE.Domain.DTO.Auth;
using System.Security.Claims;

namespace MetroTicketBE.Application.IService
{
    public interface IAuthService
    {
        Task<ResponseDTO> RegisterCustomer(RegisterCustomerDTO registerCustomerDTO);
        Task<ResponseDTO> LoginUser(LoginDTO loginDTO);
        Task<ResponseDTO> RegisterCustomerByGoogle(RegisterCustomerByGoogleDTO registerCustomerByGoogleDTO);
        Task<ResponseDTO> LoginUserByGoogle(LoginByGoogleDTO loginByGoogleDTO);
        Task<ResponseDTO> SendVerifyEmail(string email);
        Task<ResponseDTO> VerifyEmail(string userId, string token);
        Task<ResponseDTO> Logout(ClaimsPrincipal user);
        Task<ResponseDTO> SetStaffRole(string email);
        Task<ResponseDTO> SetManagerRole(string email);
        Task<ResponseDTO> SetAdminRole(string email);
        Task<ResponseDTO> CreateStaffAsync(RegisterCustomerDTO dto);
        Task<ResponseDTO> CreateManagerAsync(RegisterCustomerDTO dto);
        Task<ResponseDTO> CreateAdminAsync(RegisterCustomerDTO dto);
        Task<ResponseDTO> ChangPassword(ClaimsPrincipal user, ChangePasswordDTO changePasswordDTO);
        Task<ResponseDTO> SendResetPasswordEmail(SendResetPasswordDTO sendResetPasswordDTO);
        Task<ResponseDTO> ResetPassword(ResetPasswordDTO resetPasswordDTO);
        Task<ResponseDTO> RemoveStaff(string email);
    }
}
