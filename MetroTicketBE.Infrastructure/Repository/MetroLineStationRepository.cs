﻿using MetroTicketBE.Domain.Entities;
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
        
        public async Task<List<MetroLineStation>> GetStationByMetroLineIdAsync(Guid metroLineId, bool? isActive = null)
        {
            var query =  _context.MetroLineStations
                .Where(mls => mls.MetroLineId == metroLineId);

            if (isActive.HasValue)
            {
                query = query.Where(mls => mls.IsActive == isActive.Value);
            }
            
            query = query.Include(mls => mls.Station);
            query = query.Include(mls => mls.MetroLine);
            
            var stationsInMetroLine = await query
                .OrderBy(mls => mls.StationOrder) // nếu muốn theo thứ tự đi
                .Select(mls => mls.Station)
                .ToListAsync();

            return await query.OrderBy(mls => mls.StationOrder).ToListAsync();
        }
        
        public async Task<List<Station>> GetOrderedStationsByMetroLineIdAsync(Guid metroLineId, bool? isActive = null)
        {
            var query = _context.MetroLineStations
                .Where(mls => mls.MetroLineId == metroLineId);

            if (isActive.HasValue)
            {
                query = query.Where(mls => mls.IsActive == isActive.Value);
            }

            query = query.Include(mls => mls.Station);
            query = query.Include(mls => mls.MetroLine);

            return await query
                .OrderBy(mls => mls.StationOrder)
                .Select(mls => mls.Station)
                .ToListAsync();
        }
    }
}
