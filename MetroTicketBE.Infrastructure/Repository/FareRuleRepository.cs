using MetroTicketBE.Domain.Entities;
using MetroTicketBE.Infrastructure.Context;
using MetroTicketBE.Infrastructure.IRepository;
using Microsoft.EntityFrameworkCore;

namespace MetroTicketBE.Infrastructure.Repository
{
    public class FareRuleRepository : Repository<FareRule>, IFareRuleRepository
    {
        private readonly ApplicationDBContext _context;
        public FareRuleRepository(ApplicationDBContext context) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<bool> IsExistById(Guid fareRuleId)
        {
            return await _context.FareRules
                .AnyAsync(fareRule => fareRule.Id == fareRuleId);
        }
    }
}
