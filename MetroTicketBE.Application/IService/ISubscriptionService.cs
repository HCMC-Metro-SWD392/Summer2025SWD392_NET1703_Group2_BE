using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.DTO.SubscriptionTicket;

namespace MetroTicketBE.Application.IService;

public interface ISubscriptionService
{
    public Task<ResponseDTO> CreateSubscriptionAsync(CreateSubscriptionDTO dto);
}