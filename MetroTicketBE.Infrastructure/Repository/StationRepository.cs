using MetroTicketBE.Domain.DTO.Station;
using MetroTicketBE.Domain.Entities;
using MetroTicketBE.Infrastructure.Context;
using MetroTicketBE.Infrastructure.IRepository;
using Microsoft.EntityFrameworkCore;

namespace MetroTicketBE.Infrastructure.Repository
{
    public class StationRepository : Repository<Station>, IStationRepository
    {
        private readonly ApplicationDBContext _context;
        public StationRepository(ApplicationDBContext context) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public double CalculateTotalDistance(List<Guid> stationPath, List<MetroLine> allMetroLines)
        {
            var stationLookup = allMetroLines.SelectMany
                (line => line.MetroLineStations)
                .GroupBy(mls => mls.StationId)
                .ToDictionary(g => g.Key, g => g.First());

            double totalDistance = 0;

            for (int i = 0; i < stationPath.Count - 1; i++)
            {
                var currentId = stationPath[i];
                var nextId = stationPath[i + 1];

                if (stationLookup.TryGetValue(currentId, out var current) &&
                    stationLookup.TryGetValue(nextId, out var next))
                {
                    double distance = Math.Abs(next.DistanceFromStart - current.DistanceFromStart);
                    totalDistance += distance;
                }
            }

            return totalDistance;
        }

        public async Task<bool> IsExistById(Guid stationId)
        {
            return await _context.Stations
                .AnyAsync(s => s.Id == stationId);
        }

        public async Task<bool> IsExistByNameAndStationId(string stationName, Guid stationId)
        {
            var trimmedStationName = stationName.Trim();
                return await _context.Stations
                    .AnyAsync(s => s.Name.ToUpper() == stationName.ToUpper() && s.Id != stationId);
        }
        public async Task<bool> IsExistByName(string stationName)
        {
            return await _context.Stations
                .AnyAsync(s => s.Name == stationName);
        }
        public async Task<bool> IsExistByAddress(string address, Guid stationId)
        {
            var trimmedAddress = address.Trim();
            return await _context.Stations
                .AnyAsync(s => s.Address.ToUpper() == trimmedAddress.ToUpper() && s.Id != stationId);
        }
        
        public Task<string?> GetNameById(Guid stationId)
        {
            return _context.Stations
                .Where(s => s.Id == stationId)
                .Select(s => s.Name)
                .FirstOrDefaultAsync();
        }

        public async Task<int> GetOrderStationById(Guid stationId, Guid metroLineId)
        {
            return await _context.Stations
                .Where(s => s.Id == stationId)
                .Select(s => s.MetroLineStations
                .Where(mls => mls.MetroLineId == metroLineId)
                    .Select(mls => mls.StationOrder)
                    .FirstOrDefault())
                .FirstOrDefaultAsync();
        }

        public async Task<List<Station>> GetAllStationsAsync(bool? isAscending, bool? isActive = null)
        {
            var query = _context.Stations.AsQueryable();

            if (isActive.HasValue)
            {
                query = query.Where(s => s.IsActive == isActive.Value);
            }

            if (isAscending.HasValue && isAscending.Value)
            {
                return await query
                    .OrderBy(s => s.CreatedAt)
                    .Include(s => s.MetroLineStations)
                    .ThenInclude(mls => mls.MetroLine)
                    .ToListAsync();
            }

            return await query
                .OrderByDescending(s => s.CreatedAt)
                .Include(s => s.MetroLineStations).ThenInclude(mls => mls.MetroLine)
                .ToListAsync();
        }

        public IQueryable<Station> GetAllStationDTOAsync(bool? isAscending, bool? isActive = null)
        {
            var stationsQuery = _context.Stations.AsQueryable();

            if (isAscending.HasValue && isAscending.Value )
            {
                stationsQuery = stationsQuery.Where(s => isActive == null || s.IsActive == isActive).OrderBy(s => s.CreatedAt);
            }
            else
            {
                stationsQuery = stationsQuery.Where(s => isActive == null || s.IsActive == isActive).OrderByDescending(s => s.CreatedAt);
            }

            return stationsQuery;
        }

        public async Task<List<Station>> SearchStationsByName(string? name, bool? isActive = null)
        {
            
            if (name is null)
            {
                return await _context.Stations.Where(s => isActive == null || s.IsActive == isActive.Value)
                    .ToListAsync();
            }
        
            return await _context.Stations
                .Where(s => s.Name.ToLower().Contains(name.ToLower())  && 
                            (isActive == null || s.IsActive == isActive.Value))
                .Include(s => s.MetroLineStations).ThenInclude(mls => mls.MetroLine)
                .ToListAsync();
        }
        
    }
}
