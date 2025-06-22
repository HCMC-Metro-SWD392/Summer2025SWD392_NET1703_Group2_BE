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

        public async Task<Ticket?> GetByIdAsync(Guid ticketId)
        {
            return await _context.Tickets
                .Include(t => t.TicketRoute)
                .ThenInclude(tr => tr.StartStation)
                .Include(t => t.TicketRoute)
                .ThenInclude(tr => tr.EndStation)
                .Include(t => t.SubscriptionTicket)
                .ThenInclude(st => st.StartStation)
                .Include(t => t.SubscriptionTicket)
                .ThenInclude(st => st.EndStation)
                .FirstOrDefaultAsync(t => t.Id == ticketId);
        }

        public async Task<Ticket?> GetByQrCodeAsync(string qrCode)
        {
            return await _context.Tickets
                       .Include(t => t.TicketRoute)
                       .ThenInclude(tr => tr.StartStation)
                       .Include(t => t.TicketRoute)
                       .ThenInclude(tr => tr.EndStation)
                       .Include(t => t.SubscriptionTicket)
                       .ThenInclude(st => st.StartStation)
                       .Include(t => t.SubscriptionTicket)
                       .ThenInclude(st => st.EndStation)
                       .FirstOrDefaultAsync(t => t.QrCode == qrCode);
        }

        public async Task<Ticket?> GetTicketBySerialAsync(string serial)
        {
            return await _context.Tickets
                .Include(t => t.TicketRoute)
                .ThenInclude(tr => tr.StartStation)
                .Include(t => t.TicketRoute)
                .ThenInclude(tr => tr.EndStation)
                .Include(t => t.SubscriptionTicket)
                .ThenInclude(st => st.StartStation)
                .Include(t => t.SubscriptionTicket)
                .ThenInclude(st => st.EndStation)
                .FirstOrDefaultAsync(t => t.TicketSerial == serial);
        }
    }
}
