using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.Enum;

namespace MetroTicketBE.Application.IService;

public interface ILogService
{
    Task AddLogAsync(LogType logType, string userId, string entityName, string? additionalInfo);
    Task<ResponseDTO> GetAllLogs(int pageNumber, int pageSize);
    Task<ResponseDTO> GetLogsByCreatedAtRange(DateTime startDate, DateTime endDate);
    Task<ResponseDTO> GetLogsByLogType(LogType logType);
    Task<ResponseDTO> GetLogsByUserId(string userId);
}