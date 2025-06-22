using MetroTicketBE.Domain.Entities;
using MetroTicketBE.Domain.Enum;
using MetroTicketBE.Infrastructure.Context;
using MetroTicketBE.Infrastructure.IRepository;
using Microsoft.EntityFrameworkCore;

namespace MetroTicketBE.Infrastructure.Repository
{
    public class FormRequestRepository : Repository<FormRequest>, IFormRequestRepository
    {
        private readonly ApplicationDBContext _context;
        public FormRequestRepository(ApplicationDBContext context) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<FormRequest> GetByIdAsync(Guid Id)
        {
            return await _context.FormRequests
                .Include(fr => fr.FormAttachments)
                .FirstOrDefaultAsync(fr => fr.Id == Id)
                ?? throw new KeyNotFoundException($"FormRequest with ID {Id} not found.");
        }

        public async Task<bool> IsPendingFormRequest(string userId)
        {
            return await _context.FormRequests
                .AnyAsync(fr => fr.SenderId == userId && fr.Status == FormStatus.Pending);
        }
    }
}
