using MetroTicketBE.Application.IService;
using MetroTicketBE.Domain.Constants;
using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.DTO.Ticket;
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
        [Route("get-all-tickets")]
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
            var response = await _ticketService.GetAllTickets(User, filterOn, filterQuery, fromPrice, toPrice, sortBy, isAcsending, status, pageNumber, pageSize);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet]
        [Route("get-ticket/{serial}")]
        [Authorize(Roles = StaticUserRole.StaffManagerAdmin)]
        public async Task<ActionResult<ResponseDTO>> GetTicketBySerialAsync([FromRoute] string serial)
        {
            var response = await _ticketService.GetTicketBySerial(serial);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut]
        [Route("change-ticket-route-status/{ticketId:guid}")]
        [Authorize(Roles = StaticUserRole.StaffManagerAdmin)]
        public async Task<ActionResult<ResponseDTO>> ChangeTicketRouteStatus([FromRoute] Guid ticketId)
        {
            var response = await _ticketService.ChangeTicketRouteStatus(ticketId);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost]
        [Route("create-ticket-for-special-case")]
        [Authorize(Roles = StaticUserRole.StaffManagerAdmin)]
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
        [Route("check-in-ticket-process")]
        [Authorize(Roles = StaticUserRole.StaffManagerAdmin)]
        public async Task<ActionResult<ResponseDTO>> CheckInTicketProcess([FromBody] TicketProcessDTO ticketProcessDTO)
        {
            var response = await _ticketService.CheckInTicketProcess(ticketProcessDTO.QrCode, ticketProcessDTO.StationId);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut]
        [Route("check-out-ticket-process")]
        [Authorize(Roles = StaticUserRole.StaffManagerAdmin)]
        public async Task<ActionResult<ResponseDTO>> CheckOutTicketProcess([FromBody] TicketProcessDTO ticketProcessDTO)
        {
            var response = await _ticketService.CheckOutTicketProcess(ticketProcessDTO.QrCode, ticketProcessDTO.StationId);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet]
        [Route("get-qr-code/{ticketId:guid}")]
        [Authorize]
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
            var response = await _ticketService.GetORCode(User, ticketId);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet]
        [Route("check-exist-ticket-range")]
        [Authorize]
        public async Task<ActionResult<ResponseDTO>> CheckExistTicketRange([FromQuery] Guid startStaionId, [FromQuery] Guid endStationId)
        {
            if (startStaionId == Guid.Empty || endStationId == Guid.Empty)
            {
                return BadRequest(new ResponseDTO
                {
                    IsSuccess = false,
                    StatusCode = 400,
                    Message = "Mã trạm không hợp lệ"
                });
            }
            var response = await _ticketService.CheckExistTicketRange(User, startStaionId, endStationId);
            return StatusCode(response.StatusCode, response);
        }
    }
}
