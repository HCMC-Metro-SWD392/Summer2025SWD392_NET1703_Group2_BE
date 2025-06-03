using MetroTicketBE.Domain.Entities;

namespace MetroTicketBE.Infrastructure.IRepository
{
    public interface ITicketRouteRepository : IRepository<TicketRoute>
    {
        Task<TicketRoute?> GetTicketRouteByStartAndEndStation(Guid StartStation, Guid EndStation);
    }
}
