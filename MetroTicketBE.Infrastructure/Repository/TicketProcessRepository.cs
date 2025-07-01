using MetroTicketBE.Domain.Entities;
using MetroTicketBE.Infrastructure.Context;
using MetroTicketBE.Infrastructure.IRepository;

namespace MetroTicketBE.Infrastructure.Repository
{
    public class TicketProcessRepository : Repository<TicketProcess>, ITicketProcessRepository
    {
        private readonly ApplicationDBContext _context;
        public TicketProcessRepository(ApplicationDBContext context) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
    }
}
