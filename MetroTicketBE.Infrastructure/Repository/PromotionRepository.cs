using MetroTicketBE.Domain.Entities;
using MetroTicketBE.Infrastructure.Context;
using MetroTicketBE.Infrastructure.IRepository;
using Microsoft.EntityFrameworkCore;

namespace MetroTicketBE.Infrastructure.Repository
{
    public class PromotionRepository : Repository<Promotion>, IPromotionRepository
    {
        private readonly ApplicationDBContext _context;
        public PromotionRepository(ApplicationDBContext context) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Promotion?> GetByCodeAsync(string code)
        {
            return await _context.Promotions.
                Include(p => p.Transactions)
                .FirstOrDefaultAsync(p => p.Code == code);
        }

        public async Task<Promotion?> GetByIdAsync(Guid promotionId)
        {
            return await _context.Promotions.
                Include(p => p.Transactions)
                .FirstOrDefaultAsync(p => p.Id == promotionId);
        }

        public Task<bool> IsExistByCode(string code)
        {
            return _context.Promotions
                .AnyAsync(p => p.Code == code);
        }

        public Task<bool> IsExistByCodeExceptId(string code, Guid exceptId)
        {
            return _context.Promotions
                .AnyAsync(p => p.Code == code && p.Id != exceptId);
        }
    }
}
