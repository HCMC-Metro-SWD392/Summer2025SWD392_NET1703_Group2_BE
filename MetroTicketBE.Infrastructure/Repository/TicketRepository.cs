using MetroTicketBE.Domain.Entities;
using MetroTicketBE.Infrastructure.Context;
using MetroTicketBE.Infrastructure.IRepository;
using Microsoft.EntityFrameworkCore;

namespace MetroTicketBE.Infrastructure.Repository
{
    public class TicketRepository : Repository<Ticket>, ITicketRepository
    {
        private readonly ApplicationDBContext _context;
        public TicketRepository(ApplicationDBContext context) : base(context)
        {
            _context = context;
        }

        public Task<Ticket?> GetByIdAsync(Guid ticketId)
        {
            return _context.Tickets
                .Include(t => t.TicketRoute)
                .ThenInclude(tr => tr.StartStation)
                .Include(t => t.TicketRoute)
                .ThenInclude(tr => tr.EndStation)
                .FirstOrDefaultAsync(t => t.Id == ticketId);
        }

        public Task<Ticket?> GetTicketBySerialAsync(string serial)
        {
            return _context.Tickets
                .Include(t => t.TicketRoute)
                .ThenInclude(tr => tr.StartStation)
                .Include(t => t.TicketRoute)
                .ThenInclude(tr => tr.EndStation)
                .FirstOrDefaultAsync(t => t.TicketSerial == serial);
        }
    }
}
