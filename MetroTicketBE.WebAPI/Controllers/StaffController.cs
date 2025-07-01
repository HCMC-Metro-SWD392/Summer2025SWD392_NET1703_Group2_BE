using MetroTicketBE.Application.IService;
using MetroTicketBE.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MetroTicketBE.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StaffController : ControllerBase
    {
        private readonly IStaffService _staffService;
        public StaffController(IStaffService staffService)
        {
            _staffService = staffService ?? throw new ArgumentNullException(nameof(staffService));
        }

        [HttpGet]
        [Route("GetAllStaff")]
        [Authorize(Roles = StaticUserRole.ManagerAdmin)]
        public async Task<IActionResult> GetAllStaff()
        {
            var response = await _staffService.GetAllStaff();
            return StatusCode(response.StatusCode, response);
        }
    }
}

