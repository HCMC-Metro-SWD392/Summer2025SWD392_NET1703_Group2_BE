﻿using MetroTicketBE.Application.IService;
using MetroTicketBE.Domain.Constants;
using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.DTO.SubscriptionTicket;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MetroTicketBE.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubscriptionTicketController: ControllerBase
{
    private readonly ISubscriptionService _subscriptionService;
    public SubscriptionTicketController(ISubscriptionService subscriptionService)
    {
        _subscriptionService = subscriptionService ?? throw new ArgumentNullException(nameof(subscriptionService));
    }
    
    [HttpPost]
    [Route("create-subscription-ticket")]
    [Authorize]
    public async Task<IActionResult> CreateSubscriptionAsync([FromBody] CreateSubscriptionDTO dto)
    {
        if (dto == null)
        {
            return BadRequest(new ResponseDTO
            {
                IsSuccess = false,
                StatusCode = 400,
                Message = "Dữ liệu không hợp lệ"
            });
        }
    
        var response = await _subscriptionService.CreateSubscriptionTicketAsync(dto);
        if (response.IsSuccess)
        {
            return Ok(response);
        }
        
        return BadRequest(response);
    }
    
    [HttpGet]
    [Route("all")]
    [Authorize(Roles = StaticUserRole.ManagerAdmin)]
    public async Task<IActionResult> GetAllSubscriptionsAsync()
    {
        var response = await _subscriptionService.GetAllSubscriptionsAsync();
        if (response.IsSuccess)
        {
            return Ok(response);
        }
        
        return BadRequest(response);
    }
    
    [HttpGet]
    [Route("{id:guid}")]
    [Authorize(Roles = StaticUserRole.ManagerAdmin)]
    public async Task<IActionResult> GetSubscriptionAsync(Guid id)
    {
        var response = await _subscriptionService.GetSubscriptionAsync(id);
        if (response.IsSuccess)
        {
            return Ok(response);
        }
        return NotFound(response);
    }
    
    [HttpGet]
    [Route("by-station/{startStationId:guid}/{endStationId:guid}/{ticketTypeId:guid}")]
    [Authorize]
    public async Task<IActionResult> GetSubscriptionByStationAsync(Guid startStationId, Guid endStationId, Guid ticketTypeId)
    {
        var response = await _subscriptionService.GetSubscriptionByStationAsync(startStationId, endStationId, ticketTypeId);
        if (response.IsSuccess)
        {
            return Ok(response);
        }
        return NotFound(response);
    }

    [HttpPut]
    [Route("update/{id:guid}")]
    [Authorize(Roles = StaticUserRole.ManagerAdmin)]
    public async Task<IActionResult> UpdateSubscriptionAsync(Guid id, [FromBody] UpdateSubscriptionDTO dto)
    {
        var response = await _subscriptionService.UpdateSubscriptionAsync(id, dto);
        if (response.IsSuccess)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }
}