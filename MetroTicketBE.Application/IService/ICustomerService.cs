using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.DTO.Customer;

namespace MetroTicketBE.Application.IService;

public interface ICustomerService
{
    public Task<ResponseDTO> GetCustomerByIdAsync(Guid customerId);
    public Task<ResponseDTO> UpdateCustomerAsync(Guid customerId, UpdateCustomerDTO updateCustomerDTO);
    public Task<ResponseDTO> GetCustomerByUserIdAsync(string userId);
    public Task<ResponseDTO> GetCustomerByEmailAsync(string email);
}