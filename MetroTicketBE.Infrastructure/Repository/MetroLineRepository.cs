using MetroTicketBE.Domain.Entities;
using MetroTicketBE.Infrastructure.Context;
using MetroTicketBE.Infrastructure.IRepository;
using Microsoft.EntityFrameworkCore;

namespace MetroTicketBE.Infrastructure.Repository
{
    public class MetroLineRepository : Repository<MetroLine>, IMetroLineRepository
    {
        private readonly ApplicationDBContext _context;

        public MetroLineRepository(ApplicationDBContext context) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<List<MetroLine>> GetAllListAsync()
        {
            return await _context.MetroLines.ToListAsync();
        }

        public async Task<bool> IsExistByMetroLineNumber(int metroLineNumber)
        {
            return await _context.MetroLines
                .AnyAsync(metroLine => metroLine.MetroLineNumber == metroLineNumber);
        }
    }
}
