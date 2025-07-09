using MetroTicket.Domain.Entities;
using MetroTicketBE.Domain.Enum;

namespace MetroTicketBE.Infrastructure.IRepository;

public interface ILogRepository: IRepository<Log>
{
    Task<List<Log>> GetAllLogs(int pageNumber, int pageSize);
    Task<List<Log>> GetByCreatedAtRange(DateTime startDate, DateTime endDate);
    Task<List<Log>> GetByUserIdAsync(string userId);
    Task<List<Log>> GetByLogTypeAsync(LogType logType);
}