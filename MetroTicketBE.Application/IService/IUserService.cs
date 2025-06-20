using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.DTO.Customer;
using MetroTicketBE.Domain.Enum;

namespace MetroTicketBE.Application.IService;

public interface IUserService
{
    Task<ResponseDTO> GetUserByIdAsync(string userId);
    Task<ResponseDTO> UpdateUserAsync(string userId, UpdateUserDTO userDTO);
    Task<ResponseDTO> CreateStaffAsync(RegisterCustomerDTO dto, UserRole role);
}