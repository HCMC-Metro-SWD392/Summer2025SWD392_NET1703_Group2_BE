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

        public async Task<bool> IsExistByName(string stationName)
        {
            return await _context.Stations
                .AnyAsync(s => s.Name == stationName);
        }

        public async Task<bool> IsExistByAddress(string address)
        {
            return await _context.Stations
                .AnyAsync(s => s.Address == address);
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

        public async Task<List<Station>> GetAllStationsAsync(bool? isAscending)
        {
            if (isAscending.HasValue && isAscending.Value == true)
            {
                return await _context.Stations
                    .OrderBy(s => s.CreatedAt)
                    .Include(s => s.MetroLineStations).ThenInclude(mls => mls.MetroLine)
                    .ToListAsync();
            }

            return await _context.Stations
                    .OrderByDescending(s => s.CreatedAt)
                    .Include(s => s.MetroLineStations).ThenInclude(mls => mls.MetroLine)
                .ToListAsync();
        }

        public async Task<List<Station>> SearchStationsByName(string? name)
        {
            
            if (name is null)
            {
                return await _context.Stations
                    .ToListAsync();
            }
        
            return await _context.Stations
                .Where(s => s.Name.ToLower().Contains(name.ToLower()))
                .Include(s => s.MetroLineStations).ThenInclude(mls => mls.MetroLine)
                .ToListAsync();
        }
        
    }
}
