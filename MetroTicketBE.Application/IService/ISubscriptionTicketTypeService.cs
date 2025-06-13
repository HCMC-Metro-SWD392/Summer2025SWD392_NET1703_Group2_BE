using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.DTO.SubscriptionTicketType;

namespace MetroTicketBE.Application.IService;

public interface ISubscriptionTicketTypeService
{
    Task<ResponseDTO> GetAllAsync();
    Task<ResponseDTO> GetByIdAsync(Guid id);
    Task<ResponseDTO> GetByNameAsync(string name);
    Task<ResponseDTO> CreateAsync(CreateSubscriptionTicketTypeDTO createSubscriptionTicketTypeDTO);
}