using MetroTicketBE.Application.IService;
using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.DTO.TicketRoute;
using MetroTicketBE.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MetroTicketBE.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketRouteController : ControllerBase
    {
        private readonly ITicketRouteService _ticketRouteService;
        public TicketRouteController(ITicketRouteService ticketRouteService)
        {
            _ticketRouteService = ticketRouteService ?? throw new ArgumentNullException(nameof(ticketRouteService));
        }
        [HttpPost]
        [Route("create-ticket-route")]
        public async Task<ActionResult<ResponseDTO>> CreateTicketRoute([FromBody] CreateTicketRouteDTO createTicketRouteDTO)
        {
            var response = await _ticketRouteService.CraeteTicketRoute(createTicketRouteDTO);
            return StatusCode(response.StatusCode, response);
        }
        [HttpGet("get-ticket-route-by-from-to/{startStationId:guid}/{endStationId:guid}")]
        public async Task<ActionResult<ResponseDTO>> GetTicketRouteByFromTo([FromRoute] Guid startStationId, Guid endStationId)
        {
            var response = await _ticketRouteService.GetTicketRouteByFromToAsync(startStationId, endStationId);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("get-all-ticket-routes")]
        public async Task<ActionResult<ResponseDTO>> GetAllTicketRoutesInActiveAsync(
            [FromQuery] string? filterOn,
            [FromQuery] string? filterQuery,
            [FromQuery] double? fromPrice,
            [FromQuery] double? toPrice,
            [FromQuery] string? sortBy,
            [FromQuery] bool? isAcsending,
            [FromQuery] TicketRoutStatus status = TicketRoutStatus.Inactive,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var response = await _ticketRouteService.GetAllTicketRoutesInActiveAsync(User, filterOn, filterQuery, fromPrice, toPrice, sortBy, isAcsending, status, pageNumber, pageSize);
            return StatusCode(response.StatusCode, response);
        }
    }
}
