using MetroTicketBE.Application.IService;
using MetroTicketBE.Application.Service;
using MetroTicketBE.Domain.Constants;
using MetroTicketBE.Domain.DTO.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MetroTicketBE.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashBoardController : ControllerBase
    {
        private readonly IDashBoardService _dashBoardService;
        public DashBoardController(IDashBoardService dashBoardService)
        {
            _dashBoardService = dashBoardService ?? throw new ArgumentNullException(nameof(dashBoardService));
        }
        [HttpGet]
        [Route("revenue-month")]
        [Authorize(Roles = StaticUserRole.ManagerAdmin)]
        public async Task<ActionResult<ResponseDTO>> ViewRevenueMonth([FromQuery] int month)
        {
            var response = await _dashBoardService.ViewRevenueMonth(month);
            return StatusCode(response.StatusCode, response);
        }
        [HttpGet]
        [Route("revenue/overtime")]
        [Authorize(Roles = StaticUserRole.ManagerAdmin)]
        public async Task<ActionResult<ResponseDTO>> ViewRevenueOverTime([FromQuery] DateTime dateFrom, [FromQuery] DateTime dateTo)
        {
            var response = await _dashBoardService.ViewRevenueOverTime(dateFrom, dateTo);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet]
        [Route("revenue-year")]
        [Authorize(Roles = StaticUserRole.ManagerAdmin)]
        public async Task<ActionResult<ResponseDTO>> ViewRevenueYear([FromQuery] int year)
        {
            var response = await _dashBoardService.ViewRevenueYear(year);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet]
        [Route("ticket-statistics")]
        [Authorize(Roles = StaticUserRole.ManagerAdmin)]
        public async Task<ActionResult<ResponseDTO>> ViewTicketStatistics(
           [FromQuery] DateTime dateFrom = default,
           [FromQuery] DateTime dateTo = default,
           [FromQuery] bool? isAccendingCreated = false,
           [FromQuery] int pageNumber = 1,
           [FromQuery] int pageSize = 10)
        {
            dateFrom = dateFrom == default ? DateTime.UtcNow : dateFrom;
            var response = await _dashBoardService.ViewTicketStatistics(dateFrom, dateTo, isAccendingCreated, pageNumber, pageSize);
            return StatusCode(response.StatusCode, response);
        }
    }
}
