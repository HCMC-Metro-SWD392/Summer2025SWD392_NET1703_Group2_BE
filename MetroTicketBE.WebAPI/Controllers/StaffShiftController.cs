using MetroTicketBE.Application.IService;
using MetroTicketBE.Domain.Constants;
using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.DTO.StaffShift;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MetroTicketBE.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StaffShiftController: ControllerBase
{
    private readonly IStaffShiftService _staffShiftService;
    public StaffShiftController(IStaffShiftService staffShiftService)
    {
        _staffShiftService = staffShiftService ?? throw new ArgumentNullException(nameof(staffShiftService));
    }
    [HttpGet("get-all")]
    [Authorize(Roles = StaticUserRole.ManagerAdmin)]
    public async Task<IActionResult> GetAllStaffShifts()
    {
        var response = await _staffShiftService.GetAllStaffShifts();
        if (response.IsSuccess)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }
    [HttpPost("create")]
    [Authorize(Roles = StaticUserRole.ManagerAdmin)]
    public async Task<IActionResult> CreateStaffShift([FromBody] CreateShiftDTO createShiftDTO)
    {
        if (createShiftDTO == null)
        {
            return BadRequest(new ResponseDTO
            {
                IsSuccess = false,
                StatusCode = 400,
                Message = "Dữ liệu không hợp lệ"
            });
        }
        
        var response = await _staffShiftService.CraeteStaffShift(createShiftDTO);
        if (response.IsSuccess)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }
}