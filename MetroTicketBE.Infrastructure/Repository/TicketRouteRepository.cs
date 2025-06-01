using MetroTicketBE.Domain.Entities;
using MetroTicketBE.Infrastructure.Context;
using MetroTicketBE.Infrastructure.IRepository;

namespace MetroTicketBE.Infrastructure.Repository
{
    public class TicketRouteRepository : Repository<TicketRoute>, ITicketRouteRepository
    {
        private readonly ApplicationDBContext _context;
        public TicketRouteRepository(ApplicationDBContext context) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

    }
}
