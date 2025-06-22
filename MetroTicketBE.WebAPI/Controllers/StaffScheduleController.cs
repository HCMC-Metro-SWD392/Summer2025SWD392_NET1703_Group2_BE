using MetroTicketBE.Application.IService;
using MetroTicketBE.Domain.DTO.StaffSchedule;
using Microsoft.AspNetCore.Mvc;

namespace MetroTicketBE.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StaffScheduleController: ControllerBase
{
    private readonly IStaffScheduleService _staffScheduleService;
    public StaffScheduleController(IStaffScheduleService staffScheduleService)
    {
        _staffScheduleService = staffScheduleService ?? throw new ArgumentNullException(nameof(staffScheduleService));
    }
    [HttpGet("schedules")]
    public async Task<IActionResult> GetAllSchedules([FromQuery] DateOnly startDate, [FromQuery] DateOnly endDate)
    {
        var response = await _staffScheduleService.GetAllSchedules(startDate, endDate);
        if (response.IsSuccess)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }
    [HttpPost("create")]
    public async Task<IActionResult> CreateStaffSchedule([FromBody] CreateStaffScheduleDTO dto)
    {
        var response = await _staffScheduleService.CreateStaffSchedule(dto);
        if (response.IsSuccess)
        {
            return CreatedAtAction(nameof(GetAllSchedules), new { startDate = dto.WorkingDate, endDate = dto.WorkingDate }, response);
        }
        return BadRequest(response);
    }
    [HttpGet("schedules-by-station")]
    public async Task<IActionResult> GetSchedulesByStationIdAndDate([FromQuery] Guid stationId, [FromQuery] DateOnly workingDate)
    {
        var response = await _staffScheduleService.GetSchedulesByStationIdAndDate(stationId, workingDate);
        if (response.IsSuccess)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }
}