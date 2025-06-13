using MetroTicketBE.Domain.Entities;
using MetroTicketBE.Infrastructure.Context;
using MetroTicketBE.Infrastructure.IRepository;
using Microsoft.EntityFrameworkCore;

namespace MetroTicketBE.Infrastructure.Repository;

public class SubscriptionTicketTypeRepository: Repository<SubscriptionTicketType> , ISubscriptionTicketTypeRepository
{
    private readonly ApplicationDBContext _context;
    
    public SubscriptionTicketTypeRepository(ApplicationDBContext context) : base(context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }
    public async Task<SubscriptionTicketType?> GetByNameAsync(string name)
    {
        return await _context.SubscriptionTicketTypes
            .FirstOrDefaultAsync(st => st.Name == name);
    }

}