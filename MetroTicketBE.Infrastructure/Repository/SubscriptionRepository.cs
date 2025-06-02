using MetroTicketBE.Domain.Entities;
using MetroTicketBE.Infrastructure.Context;
using MetroTicketBE.Infrastructure.IRepository;

namespace MetroTicketBE.Infrastructure.Repository;

public class SubscriptionRepository: Repository<SubscriptionTicket>, ISubscriptionRepository
{
    private readonly ApplicationDBContext _context;

    public SubscriptionRepository(ApplicationDBContext context) : base(context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }


    public new async Task<SubscriptionTicket> AddSubscriptionAsync(SubscriptionTicket subscription)
    {
        var entityEntry = await _context.SubscriptionTicket.AddAsync(subscription);
        return entityEntry.Entity;
    }
}