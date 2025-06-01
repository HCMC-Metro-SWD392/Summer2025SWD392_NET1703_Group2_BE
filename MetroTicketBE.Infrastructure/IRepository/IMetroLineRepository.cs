using MetroTicketBE.Domain.Entities;

namespace MetroTicketBE.Infrastructure.IRepository
{
    public interface IMetroLineRepository : IRepository<MetroLine>
    {
        Task<List<MetroLine>> GetAllListAsync();
    }
}
