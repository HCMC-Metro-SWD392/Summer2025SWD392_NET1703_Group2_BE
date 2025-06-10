using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.DTO.SubscriptionTicket;

namespace MetroTicketBE.Application.IService;

public interface ISubscriptionService
{
    public Task<ResponseDTO> CreateSubscriptionAsync(CreateSubscriptionDTO dto);
    public Task<ResponseDTO> GetAllSubscriptionsAsync(bool getAll = false);
    public Task<ResponseDTO> UpdateSubscriptionAsync(Guid id, UpdateSubscriptionDTO dto);
    public Task<ResponseDTO> GetSubscriptionAsync(Guid id);
    
}