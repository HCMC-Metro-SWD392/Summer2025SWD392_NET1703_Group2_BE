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

        public async Task<FareRule?> GetByIdAsync(Guid fareRuleId)
        {
            return await _context.FareRules
                .FirstOrDefaultAsync(fareRule => fareRule.Id == fareRuleId);
        }

        public async Task<bool> IsExistById(Guid fareRuleId)
        {
            return await _context.FareRules
                .AnyAsync(fareRule => fareRule.Id == fareRuleId);
        }

        public async Task<int> CalculatePriceFromDistance(double? distance)
        {
            return (await this.GetAllAsync())
            .Where(fr => fr.MinDistance <= distance && fr.MaxDistance >= distance)
            .Select(fr => fr.Fare).FirstOrDefault();
        }
    }
}
