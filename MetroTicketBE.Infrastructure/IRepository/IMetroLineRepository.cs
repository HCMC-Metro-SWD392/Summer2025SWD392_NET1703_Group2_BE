using MetroTicketBE.Domain.Entities;

namespace MetroTicketBE.Infrastructure.IRepository
{
    public interface IMetroLineRepository : IRepository<MetroLine>
    {
        Task<List<MetroLine>> GetAllListAsync();
        Task<MetroLine?> GetByIdAsync(Guid id);
        Task<bool> IsExistByMetroLineNumber(string metroLineNumber, Guid? currentMetroLineId);
        Task<bool> IsExistById(Guid id);
        Task<bool> IsSameMetroLine(Guid stationOneId, Guid stationTwoId);
    }
}
