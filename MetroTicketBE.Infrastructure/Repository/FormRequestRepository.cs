using MetroTicketBE.Domain.Entities;
using MetroTicketBE.Infrastructure.Context;
using MetroTicketBE.Infrastructure.IRepository;

namespace MetroTicketBE.Infrastructure.Repository
{
    public class FormRequestRepository : Repository<FormRequest>, IFormRequestRepository
    {
        private readonly ApplicationDBContext _context;
        public FormRequestRepository(ApplicationDBContext context) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
    }
}
