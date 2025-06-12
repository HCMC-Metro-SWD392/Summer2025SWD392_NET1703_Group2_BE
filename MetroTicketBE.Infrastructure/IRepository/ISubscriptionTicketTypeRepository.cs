using MetroTicketBE.Domain.Entities;

namespace MetroTicketBE.Infrastructure.IRepository;

public interface ISubscriptionTicketTypeRepository: IRepository<SubscriptionTicketType>
{
    Task<SubscriptionTicketType?> GetByNameAsync(string name);
}