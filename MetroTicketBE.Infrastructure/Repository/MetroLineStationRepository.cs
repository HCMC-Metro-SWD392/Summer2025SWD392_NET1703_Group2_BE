using MetroTicketBE.Domain.Entities;
using MetroTicketBE.Infrastructure.Context;
using MetroTicketBE.Infrastructure.IRepository;
using Microsoft.EntityFrameworkCore;

namespace MetroTicketBE.Infrastructure.Repository
{
    public class MetroLineStationRepository : Repository<MetroLineStation>, IMetroLineStationRepository
    {
        private readonly ApplicationDBContext _context;
        public MetroLineStationRepository(ApplicationDBContext context) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task<bool> IsExistByOrderNumer(Guid metroLineId, Guid stationId,int stationOrder)
        {
            return await _context.MetroLineStations.
                AnyAsync(mls => mls.MetroLineId == metroLineId && mls.StationId == stationId && mls.StationOrder == stationOrder);;
        }
        
        public async Task<List<Station>> GetStationByMetroLineIdAsync(Guid metroLineId)
        {
            var stationsInMetroLine = await _context.MetroLineStations
                .Where(mls => mls.MetroLineId == metroLineId)
                .OrderBy(mls => mls.StationOrder) // nếu muốn theo thứ tự đi
                .Select(mls => mls.Station)
                .ToListAsync();
            return stationsInMetroLine;
        }
    }
}
