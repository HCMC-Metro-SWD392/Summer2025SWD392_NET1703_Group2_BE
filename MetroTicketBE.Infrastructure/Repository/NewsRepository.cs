using MetroTicketBE.Domain.Entities;
using MetroTicketBE.Infrastructure.Context;
using MetroTicketBE.Infrastructure.IRepository;
using Microsoft.EntityFrameworkCore;

namespace MetroTicketBE.Infrastructure.Repository
{
    public class NewsRepository : Repository<News>, INewsRepository
    {
        private readonly ApplicationDBContext _context;
        public NewsRepository(ApplicationDBContext context) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<News?> GetByIdAsync(Guid newsId)
        {
            return await _context.News
                .Include(n => n.Staff)
                .ThenInclude(s => s.User)
                .FirstOrDefaultAsync(n => n.Id == newsId);
        }
    }
}
