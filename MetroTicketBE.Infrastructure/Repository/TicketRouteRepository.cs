using MetroTicketBE.Domain.Entities;
using MetroTicketBE.Infrastructure.Context;
using MetroTicketBE.Infrastructure.IRepository;
using Microsoft.EntityFrameworkCore;

namespace MetroTicketBE.Infrastructure.Repository
{
    public class TicketRouteRepository : Repository<TicketRoute>, ITicketRouteRepository
    {
        private readonly ApplicationDBContext _context;
        public TicketRouteRepository(ApplicationDBContext context) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<TicketRoute?> GetByIdAsync(Guid? id)
        {
            if (id == null) return null;
            return await _context.TicketRoutes
                .FirstOrDefaultAsync(tr => tr.Id == id);
        }

        public async Task<TicketRoute?> GetByNameAsync(string name)
        {
            return await _context.TicketRoutes
                .FirstOrDefaultAsync(tr => tr.TicketName == name);
        }

        public async Task<TicketRoute?> GetTicketRouteByStartAndEndStation(Guid StartStation, Guid EndStation)
        {
            return await _context.TicketRoutes
                .FirstOrDefaultAsync(tr => tr.StartStation.Id == StartStation &&
                                           tr.EndStation.Id == EndStation);
        }
    }
}
