using MetroTicketBE.Application.IService;
using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MetroTicketBE.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly ITicketService _ticketService;
        public TicketController(ITicketService ticketService)
        {
            _ticketService = ticketService ?? throw new ArgumentNullException(nameof(ticketService));
        }

        [HttpGet]
        [Route("get-all-ticket-routes")]
        [Authorize]
        public async Task<ActionResult<ResponseDTO>> GetAllTicketRoutesAsync(
            [FromQuery] string? filterOn,
            [FromQuery] string? filterQuery,
            [FromQuery] double? fromPrice,
            [FromQuery] double? toPrice,
            [FromQuery] string? sortBy,
            [FromQuery] bool? isAcsending,
            [FromQuery] TicketStatus status = TicketStatus.Inactive,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var response = await _ticketService.GetAllTicketRoutes(User, filterOn, filterQuery, fromPrice, toPrice, sortBy, isAcsending, status, pageNumber, pageSize);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet]
        [Route("get-ticket/{serial}")]
        public async Task<ActionResult<ResponseDTO>> GetTicketBySerialAsync([FromRoute] string serial)
        {
            var response = await _ticketService.GetTicketBySerial(serial);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut]
        [Route("change-ticket-route-status/{ticketId:guid}")]
        public async Task<ActionResult<ResponseDTO>> ChangeTicketRouteStatus([FromRoute] Guid ticketId)
        {
            var response = await _ticketService.ChangeTicketRouteStatus(ticketId);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost]
        [Route("create-ticket-for-special-case")]
        public async Task<ActionResult<ResponseDTO>> CreateTicketForSpecialCase([FromQuery] Guid subscriptionId)
        {
            if (subscriptionId == Guid.Empty)
            {
                return BadRequest(new ResponseDTO
                {
                    IsSuccess = false,
                    StatusCode = 400,
                    Message = "Mã đăng ký không hợp lệ"
                });
            }

            var response = await _ticketService.CreateTicketForSpecialCase(User, subscriptionId);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut]
        [Route("check-in-ticket-process/{stationId:guid}")]
        public async Task<ActionResult<ResponseDTO>> CheckInTicketProcess([FromQuery] string qrCode, [FromRoute] Guid stationId)
        {
            var response = await _ticketService.CheckInTicketProcess(qrCode, stationId);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut]
        [Route("check-out-ticket-process/{stationId:guid}")]
        public async Task<ActionResult<ResponseDTO>> CheckOutTicketProcess([FromQuery] string qrCode, [FromRoute] Guid stationId)
        {
            var response = await _ticketService.CheckOutTicketProcess(qrCode, stationId);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet]
        [Route("get-or-code/{ticketId:guid}")]
        public async Task<ActionResult<ResponseDTO>> GetORCode([FromRoute] Guid ticketId)
        {
            if (ticketId == Guid.Empty)
            {
                return BadRequest(new ResponseDTO
                {
                    IsSuccess = false,
                    StatusCode = 400,
                    Message = "Mã vé không hợp lệ"
                });
            }
            var response = await _ticketService.GetORCode(User,ticketId);
            return StatusCode(response.StatusCode, response);
        }
    }
}
