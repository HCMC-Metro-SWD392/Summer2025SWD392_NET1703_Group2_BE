using AutoMapper;
using MetroTicket.Domain.Entities;
using MetroTicketBE.Application.IService;
using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.DTO.Log;
using MetroTicketBE.Domain.Enum;
using MetroTicketBE.Infrastructure.IRepository;

namespace MetroTicketBE.Application.Service;

public class LogService: ILogService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    
    public LogService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }
    
    public async Task AddLogAsync(LogType logType, string userId, string entityName, string? additionalInfo)
    {
        var user = await _unitOfWork.UserManagerRepository.GetByIdAsync(userId);
        var log = new Log()
        {
            Id = Guid.NewGuid(),
            LogType = logType,
            Description = $"{user.FullName} {ConvertLogTypeToString(logType)} {entityName}: {additionalInfo}",
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };
        
        await _unitOfWork.LogRepository.AddAsync(log);
        await _unitOfWork.SaveAsync();
    }
    
    public async Task<ResponseDTO> GetAllLogs(int pageNumber, int pageSize)
    {
        var logs = await _unitOfWork.LogRepository.GetAllLogs(pageNumber, pageSize);
        return new ResponseDTO()
        {
            IsSuccess = true,
            Message = "Logs retrieved successfully",
            StatusCode = 200,
            Result = _mapper.Map<List<GetLogDTO>>(logs)
        };
    }
    
    public async Task<ResponseDTO> GetLogsByCreatedAtRange(DateTime startDate, DateTime endDate, int pageNumber = 1, int pageSize = 10)
    {
        var logs = await _unitOfWork.LogRepository.GetByCreatedAtRange(startDate, endDate, pageNumber, pageSize);
        return new ResponseDTO()
        {
            IsSuccess = true,
            Message = "Logs retrieved successfully",
            StatusCode = 200,
            Result = _mapper.Map<List<GetLogDTO>>(logs)
        };
    }

    public async Task<ResponseDTO> GetLogsByUserId(string userId, int pageNumber = 1, int pageSize = 10)
    {
        var logs = await _unitOfWork.LogRepository.GetByUserIdAsync(userId, pageNumber, pageSize);
        return new ResponseDTO()
        {
            IsSuccess = true,
            Message = "Logs retrieved successfully",
            StatusCode = 200,
            Result = _mapper.Map<List<GetLogDTO>>(logs)
        };
    }
    
    public async Task<ResponseDTO> GetLogsByLogType(LogType logType, int pageNumber = 1, int pageSize = 10)
    {
        var logs = await _unitOfWork.LogRepository.GetByLogTypeAsync(logType, pageNumber, pageSize);
        return new ResponseDTO()
        {
            IsSuccess = true,
            Message = "Logs retrieved successfully",
            StatusCode = 200,
            Result = _mapper.Map<List<GetLogDTO>>(logs)
        };
    }
    
    private static string ConvertLogTypeToString(LogType logType)
    {
        return logType switch
        {
            LogType.Create => "Thêm mới",
            LogType.Update => "Cập nhật",
            LogType.Delete => "Xóa",
            _ => throw new ArgumentOutOfRangeException(nameof(logType), logType, null)
        };
    }
    
}