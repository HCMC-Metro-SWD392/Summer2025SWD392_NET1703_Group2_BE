using MetroTicketBE.Domain.Entities;
using MetroTicketBE.Domain.Enums;
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

    public Task<bool> IsExistedByType(SubscriptionTicketType type)
    {
        if (string.IsNullOrWhiteSpace(type.ToString()))
        {
            throw new ArgumentException("Ticket name cannot be null or empty", nameof(type));
        }

        return Task.FromResult(_context.SubscriptionTicket.Any(st => st.TicketType == type));
    }
}