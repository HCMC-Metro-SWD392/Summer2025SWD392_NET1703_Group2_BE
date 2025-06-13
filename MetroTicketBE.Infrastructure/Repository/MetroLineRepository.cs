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
                .OrderBy(mts => mts.CreatedAt)
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
