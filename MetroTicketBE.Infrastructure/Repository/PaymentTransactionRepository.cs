using MetroTicketBE.Domain.Entities;
using MetroTicketBE.Infrastructure.Context;
using MetroTicketBE.Infrastructure.IRepository;
using Microsoft.EntityFrameworkCore;

namespace MetroTicketBE.Infrastructure.Repository
{
    public class PaymentTransactionRepository : Repository<PaymentTransaction>, IPaymentTransactionRepository
    {
        private readonly ApplicationDBContext _context;
        public PaymentTransactionRepository(ApplicationDBContext context) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<PaymentTransaction?> GetByIdAsync(Guid id)
        {
            return await _context.PaymentTransactions
                .Include(pt => pt.Customer)
                .Include(pt => pt.Ticket_)
                .Include(pt => pt.Promotion)
                .FirstOrDefaultAsync(pt => pt.Id == id);
        }

        public async Task<PaymentTransaction?> GetByOrderCode(string orderCode)
        {
            return await _context.PaymentTransactions
                .Include(pt => pt.Customer)
                .Include(pt => pt.Ticket_)
                .Include(pt => pt.Promotion)
                .FirstOrDefaultAsync(pt => pt.OrderCode == orderCode);
        }
    }
}
