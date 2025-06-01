using MetroTicketBE.Domain.Entities;
using MetroTicketBE.Infrastructure.Context;
using MetroTicketBE.Infrastructure.IRepository;
using Microsoft.EntityFrameworkCore;

namespace MetroTicketBE.Infrastructure.Repository
{
    public class PaymentMethodRepository : Repository<PaymentMethod>, IPaymentMethodRepository
    {
        private readonly ApplicationDBContext _context;
        public PaymentMethodRepository(ApplicationDBContext context) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task<PaymentMethod?> GetByNameAsync(string methodName)
        {
            return await _context.PaymentMethods
                .Include(pm => pm.PayOSMethods)
                .Include(pm => pm.Transactions)
                .FirstOrDefaultAsync(pm => pm.MethodName == methodName);
        }
    }
}
