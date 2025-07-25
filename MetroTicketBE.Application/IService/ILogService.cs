﻿using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.Enum;

namespace MetroTicketBE.Application.IService;

public interface ILogService
{
    Task AddLogAsync(LogType logType, string userId, string entityName, string? additionalInfo);
    Task<ResponseDTO> GetAllLogs(int pageNumber, int pageSize);
    Task<ResponseDTO> GetLogsByCreatedAtRange(DateTime startDate, DateTime endDate, int pageNumber, int pageSize);
    Task<ResponseDTO> GetLogsByLogType(LogType logType, int pageNumber, int pageSize);
    Task<ResponseDTO> GetLogsByUserId(string userId, int pageNumber, int pageSize);
}