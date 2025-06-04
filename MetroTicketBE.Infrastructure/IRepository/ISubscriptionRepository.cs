using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.Entities;
using MetroTicketBE.Domain.Enums;

namespace MetroTicketBE.Infrastructure.IRepository;

public interface ISubscriptionRepository: IRepository<SubscriptionTicket>
{
    public Task<bool> IsExistedByType(SubscriptionTicketType type);
    
}