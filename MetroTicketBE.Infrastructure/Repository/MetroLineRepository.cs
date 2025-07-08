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

        public async Task<List<MetroLine>> GetAllListAsync(bool? isActive = null)
        {
            var query = _context.MetroLines.AsQueryable();
            if (isActive.HasValue)
            {
                query = query.Where(m => m.IsActive == isActive.Value);
            }

            return await query.AsNoTracking()
                .Include(mt => mt.StartStation)
                .Include(mt => mt.EndStation)
                .Include(mt => mt.MetroLineStations)
                .ThenInclude(mts => mts.Station).ToListAsync();
        }

        public async Task<MetroLine?> GetByIdAsync(Guid id)
        {
            return await _context.MetroLines
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
        
        public async Task<bool> IsExistByMetroLineNumber(string metroLineNumber, Guid? currentMetroLineId)
        {
            var query = _context.MetroLines.Where(m => m.MetroLineNumber == metroLineNumber);
            if (currentMetroLineId.HasValue)
            {
                query = query.Where(m => m.Id != currentMetroLineId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<bool> IsSameMetroLine(Guid stationOneId, Guid stationTwoId)
        {
            return await _context.MetroLineStations
                .Where(mls1 => mls1.StationId == stationOneId)
                .Join(_context.MetroLineStations.Where(mls2 => mls2.StationId == stationTwoId),
                    mls1 => mls1.MetroLineId,
                    mls2 => mls2.MetroLineId,
                    (mls1, mls2) => mls1).AnyAsync();
        }
        
    }
}
