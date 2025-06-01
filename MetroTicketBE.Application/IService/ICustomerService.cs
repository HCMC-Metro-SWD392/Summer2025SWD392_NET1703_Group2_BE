using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.DTO.Customer;

namespace MetroTicketBE.Application.IService;

public interface ICustomerService
{
    public Task<ResponseDTO> GetCustomerByIdAsync(Guid customerId);
}