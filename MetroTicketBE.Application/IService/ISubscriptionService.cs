using MetroTicketBE.Domain.DTO.Auth;

namespace MetroTicketBE.Application.IService;

public interface ISubscriptionService
{
    public Task<ResponseDTO> AddSubscriptionAsync(Guid customerId);
}