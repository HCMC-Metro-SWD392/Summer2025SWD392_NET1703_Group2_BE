using MetroTicketBE.Domain.Entities;
using MetroTicketBE.Infrastructure.Context;
using MetroTicketBE.Infrastructure.IRepository;

namespace MetroTicketBE.Infrastructure.Repository
{
    public class FareRuleRepository : Repository<FareRule>, IFareRuleRepository
    {
        private readonly ApplicationDBContext _context;
        public FareRuleRepository(ApplicationDBContext context) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
    }
}
