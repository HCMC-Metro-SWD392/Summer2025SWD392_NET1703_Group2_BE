using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.Entities;
using MetroTicketBE.Domain.Enums;

namespace MetroTicketBE.Infrastructure.IRepository;

public interface ISubscriptionRepository: IRepository<SubscriptionTicket>
{
    public Task<bool> IsExistedByType(SubscriptionTicketType type);
    
    public Task<bool> IsExistedByName(string ticketName);
    public Task<SubscriptionTicket?> GetByNameAsync(string ticketName);
    public Task<SubscriptionTicket?> GetByIdAsync(Guid? id);
    public Task<SubscriptionTicket?> GetByStartAndEndStationAsync(Guid startStationId, Guid endStationId);
    Task<SubscriptionTicket?> GetByTicketTypeIdAsync(Guid ticketTypeId);
}