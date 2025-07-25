using MetroTicketBE.Application.IService;
using MetroTicketBE.Domain.Constants;
using MetroTicketBE.Domain.DTO.StaffSchedule;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize(Roles = StaticUserRole.StaffManagerAdmin)]
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
    [Authorize(Roles = StaticUserRole.ManagerAdmin)]
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
    [Authorize]
    public async Task<IActionResult> GetSchedulesByStationIdAndDate([FromQuery] Guid stationId, [FromQuery] DateOnly workingDate)
    {
        var response = await _staffScheduleService.GetSchedulesByStationIdAndDate(stationId, workingDate);
        if (response.IsSuccess)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }
    [HttpGet("schedules-by-staff")]
    [Authorize]
    public async Task<IActionResult> GetSchedulesByStaffIdAndDate([FromQuery] DateOnly? fromDate, [FromQuery] DateOnly? toDate)
    {
        var response = await _staffScheduleService.GetSchedulesByStaffIdAndDate(User, fromDate, toDate);
        if (response.IsSuccess)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }
    [HttpGet("get-unscheduled-staff")]
    [Authorize(Roles = StaticUserRole.StaffManagerAdmin)]
    public async Task<IActionResult> GetUnscheduledStaff([FromQuery] DateOnly workingDate, [FromQuery] Guid shiftId)
    {
        var response = await _staffScheduleService.GetUnscheduledStaff(shiftId, workingDate);
        if (response.IsSuccess)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }
    [HttpPut("assign-staff")]
    [Authorize(Roles = StaticUserRole.ManagerAdmin)]
    public async Task<IActionResult> AssignStaffToExistSchedule(Guid staffId, Guid scheduleId, Guid? workingStationId)
    {
        var response = await _staffScheduleService.AssignStaffToExistedSchedule(staffId, scheduleId, workingStationId);
        if (response.IsSuccess)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }
}