using MetroTicket.Domain.Entities;
using MetroTicketBE.Domain.Enum;

namespace MetroTicketBE.Infrastructure.IRepository;

public interface ILogRepository: IRepository<Log>
{
    Task<List<Log>> GetAllLogs(int pageNumber, int pageSize);
    Task<List<Log>> GetByCreatedAtRange(DateTime startDate, DateTime endDate, int pageNumber = 1, int pageSize = 10);
    Task<List<Log>> GetByUserIdAsync(string userId, int pageNumber = 1, int pageSize = 10);
    Task<List<Log>> GetByLogTypeAsync(LogType logType, int pageNumber = 1, int pageSize = 10);
}