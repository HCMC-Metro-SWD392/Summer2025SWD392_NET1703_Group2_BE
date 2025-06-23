using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.DTO.Customer;

namespace MetroTicketBE.Application.IService;

public interface ICustomerService
{
    public Task<ResponseDTO> GetCustomerByIdAsync(Guid customerId);
    public Task<ResponseDTO> GetCustomerByUserIdAsync(string userId);
    public Task<ResponseDTO> GetCustomerByEmailAsync(string email);
    public Task<ResponseDTO> GetAllCustomersAsync(
        string? filterOn,
        string? filterQuery,
        string? sortBy,
        bool? isAscending,
        int pageNumber,
        int pageSize);
}