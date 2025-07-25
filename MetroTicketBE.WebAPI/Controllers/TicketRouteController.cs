﻿using MetroTicketBE.Application.IService;
using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.DTO.TicketRoute;
using MetroTicketBE.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize]
        public async Task<ActionResult<ResponseDTO>> CreateTicketRoute([FromBody] CreateTicketRouteDTO createTicketRouteDTO)
        {
            var response = await _ticketRouteService.CraeteTicketRoute(createTicketRouteDTO);
            return StatusCode(response.StatusCode, response);
        }
        [HttpGet]
        [Route("get-ticket-route-by-from-to/{startStationId:guid}/{endStationId:guid}")]
        [Authorize]
        public async Task<ActionResult<ResponseDTO>> GetTicketRouteByFromTo([FromRoute] Guid startStationId, Guid endStationId)
        {
            var response = await _ticketRouteService.GetTicketRouteByFromTo(startStationId, endStationId);
            return StatusCode(response.StatusCode, response);
        }
    }
}
