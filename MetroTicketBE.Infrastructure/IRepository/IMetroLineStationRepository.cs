using MetroTicketBE.Domain.Entities;

namespace MetroTicketBE.Infrastructure.IRepository
{
    public interface IMetroLineStationRepository : IRepository<MetroLineStation>
    {
        Task<bool> IsExistByOrderNumer(Guid metroLineId, Guid stationId, int stationOrder);
        Task<List<MetroLineStation>> GetStationByMetroLineIdAsync(Guid metroLineId, bool? isActive = null);
        Task<List<Station>> GetOrderedStationsByMetroLineIdAsync(Guid metroLineId, bool? isActive = null);
    }
}
