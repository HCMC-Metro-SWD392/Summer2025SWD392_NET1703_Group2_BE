using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.Entities;

namespace MetroTicketBE.Infrastructure.IRepository;

public interface ISubscriptionRepository: IRepository<SubscriptionTicket>
{
    public Task<bool> IsExistedByName(string ticketName);
    public Task<SubscriptionTicket?> GetByNameAsync(string ticketName);
    public Task<SubscriptionTicket?> GetByIdAsync(Guid? id);

}