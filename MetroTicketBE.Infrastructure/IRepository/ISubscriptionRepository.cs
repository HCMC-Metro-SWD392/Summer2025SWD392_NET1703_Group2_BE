using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.Entities;

namespace MetroTicketBE.Infrastructure.IRepository;

public interface ISubscriptionRepository
{
    public Task<SubscriptionTicket> AddSubscriptionAsync(SubscriptionTicket subscription);

}