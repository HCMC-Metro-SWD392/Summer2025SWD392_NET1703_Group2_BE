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
            return await _context.MetroLines
                .AsNoTracking()
                .Include(mt => mt.StartStation)
                .Include(mt => mt.EndStation)
                .Include(mt => mt.MetroLineStations)
                .ThenInclude(mts => mts.Station)
                .ToListAsync();
        }

        public async Task<MetroLine?> GetByIdAsync(Guid id)
        {
            return await _context.MetroLines
                .AsNoTracking()
                .Include(mt => mt.StartStation)
                .Include(mt => mt.EndStation)
                .Include(mt => mt.MetroLineStations)
                .ThenInclude(mts => mts.Station)
                .FirstOrDefaultAsync(metroLine => metroLine.Id == id);
        }
        
        public async Task<bool> IsExistById(Guid id)
        {
            return await _context.MetroLines
                .AnyAsync(metroLine => metroLine.Id == id);
        }

        public async Task<bool> IsExistByMetroLineNumber(int metroLineNumber)
        {
            return await _context.MetroLines
                .AnyAsync(metroLine => metroLine.MetroLineNumber == metroLineNumber);
        }
    }
}
