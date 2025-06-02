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
        public async Task<bool> IsExistByOrderNumer(int stationOrder)
        {
            return await _context.MetroLineStations.AnyAsync(x => x.StationOrder == stationOrder);
        }
    }
}
