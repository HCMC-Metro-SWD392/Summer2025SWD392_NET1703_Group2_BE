using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.DTO.SubscriptionTicket;

namespace MetroTicketBE.Application.IService;

public interface ISubscriptionService
{
    //public Task<ResponseDTO> CreateSubscriptionAsync(CreateSubscriptionDTO dto);
    Task<ResponseDTO> CreateSubscriptionTicketAsync(CreateSubscriptionDTO createSubscriptionDTO);
    public Task<ResponseDTO> GetAllSubscriptionsAsync();
    public Task<ResponseDTO> UpdateSubscriptionAsync(Guid id, UpdateSubscriptionDTO dto);
    public Task<ResponseDTO> GetSubscriptionAsync(Guid id);
    public Task<ResponseDTO> GetSubscriptionByStationAsync(Guid startStationId, Guid endStationId, Guid ticketTypeId);

}