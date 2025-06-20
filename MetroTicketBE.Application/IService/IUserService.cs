using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.DTO.Customer;

namespace MetroTicketBE.Application.IService;

public interface IUserService
{
    Task<ResponseDTO> GetUserByIdAsync(string userId);
    Task<ResponseDTO> UpdateUserAsync(string userId, UpdateUserDTO userDTO);
}