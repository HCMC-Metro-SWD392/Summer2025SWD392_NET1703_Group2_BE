using MetroTicketBE.Application.IService;
using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
            [FromQuery] TicketRouteStatus status = TicketRouteStatus.Inactive,
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

        [HttpPut]
        [Route("ticket-route-process/{ticketRouteId:guid}/{stationId:guid}/{metroLineId:guid}")]
        public async Task<ActionResult<ResponseDTO>> TicketRouteProcess([FromRoute] Guid ticketRouteId, [FromRoute] Guid stationId, [FromRoute] Guid metroLineId)
        {
            var response = await _ticketService.TicketProcess(ticketRouteId, stationId, metroLineId);
            return StatusCode(response.StatusCode, response);
        }
    }
}
