using MetroTicketBE.Domain.Entities;
using MetroTicketBE.Infrastructure.Context;
using MetroTicketBE.Infrastructure.IRepository;
using Microsoft.EntityFrameworkCore;

namespace MetroTicketBE.Infrastructure.Repository;

public class SubscriptionRepository : Repository<SubscriptionTicket>, ISubscriptionRepository
{
    private readonly ApplicationDBContext _context;

    public SubscriptionRepository(ApplicationDBContext context) : base(context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<SubscriptionTicket?> GetByNameAsync(string ticketName)
    {
        return await _context.SubscriptionTickets
            .FirstOrDefaultAsync(st => st.TicketName == ticketName);
    }

    public Task<bool> IsExistedByName(string ticketName)
    {
        return _context.SubscriptionTickets
            .AnyAsync(st => st.TicketName == ticketName);
    }
}