using MetroTicketBE.Application.IService;
using MetroTicketBE.Domain.Constants;
using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.Enum;
using MetroTicketBE.Infrastructure.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MetroTicketBE.WebAPI.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = StaticUserRole.Admin)]
public class LogController: ControllerBase
{
    private readonly ILogService _logService;
    public LogController(ILogService logService)
    {
        _logService = logService ?? throw new ArgumentNullException(nameof(logService));
    }
    [HttpGet]
    [Route("all-logs")]
    
    public async Task<ActionResult<ResponseDTO>> GetAllLogs(int pageNumber = 1, int pageSize = 10)
    {
        var response = await _logService.GetAllLogs(pageNumber, pageSize);
        return StatusCode(response.StatusCode, response);
    }
    [HttpGet]
    [Route("logs-by-date-range")]
    public async Task<ActionResult<ResponseDTO>> GetLogsByDateRange(DateTime startDate, DateTime endDate)
    {
        var response = await _logService.GetLogsByCreatedAtRange(startDate, endDate);
        return StatusCode(response.StatusCode, response);
    }
    [HttpGet]
    [Route("logs-by-log-type")]
    public async Task<ActionResult<ResponseDTO>> GetLogsByLogType([FromQuery] LogType logType)
    {
        var response = await _logService.GetLogsByLogType(logType); 
        return StatusCode(response.StatusCode, response);
    }
    [HttpGet]
    [Route("logs-by-user-id")]
    public async Task<ActionResult<ResponseDTO>> GetLogsByUserId([FromQuery] string userId)
    {
        var response = await _logService.GetLogsByUserId(userId);
        return StatusCode(response.StatusCode, response);
    }
}