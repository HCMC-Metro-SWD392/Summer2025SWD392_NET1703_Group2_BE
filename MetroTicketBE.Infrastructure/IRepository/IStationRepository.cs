using MetroTicketBE.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroTicketBE.Infrastructure.IRepository
{
    public interface IStationRepository : IRepository<Station>
    {
        public double CalculateTotalDistance(List<Guid> stationPath, List<MetroLine> allMetroLines);
        Task<bool> IsExistByNameAndStationId(string stationName, Guid stationId);
        Task<bool> IsExistByName(string stationName);
        Task<bool> IsExistById(Guid stationId);
        Task<bool> IsExistByAddress(string stationAddress, Guid stationId);
        Task<string?> GetNameById(Guid stationId);
        Task<List<Station>> GetAllStationsAsync(bool? isAscending, bool? isActive);
        Task<int> GetOrderStationById(Guid stationId, Guid metroLineId);
        Task<List<Station>> SearchStationsByName(string? name, bool? isActive);
        IQueryable<Station> GetAllStationDTOAsync(bool? isAscending, bool? isActive);
    }
}
