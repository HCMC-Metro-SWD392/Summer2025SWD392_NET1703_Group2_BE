using MetroTicketBE.Domain.Entities;

namespace MetroTicketBE.Infrastructure.IRepository
{
    public interface IMetroLineStationRepository : IRepository<MetroLineStation>
    {
        Task<bool> IsExistByOrderNumer(int stationOrder);
        Task<List<Station>> GetStationByMetroLineIdAsync(Guid metroLineId);
    }
}
