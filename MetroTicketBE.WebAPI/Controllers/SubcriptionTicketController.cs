using MetroTicketBE.Application.IService;
using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.DTO.SubscriptionTicket;
using Microsoft.AspNetCore.Mvc;

namespace MetroTicketBE.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubcriptionTicketController: ControllerBase
{
    private readonly ISubscriptionService _subscriptionService;
    public SubcriptionTicketController(ISubscriptionService subscriptionService)
    {
        _subscriptionService = subscriptionService ?? throw new ArgumentNullException(nameof(subscriptionService));
    }
    
    [HttpPost]
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

        var response = await _subscriptionService.CreateSubscriptionAsync(dto);
        if (response.IsSuccess)
        {
            return Ok(response);
        }
        
        return BadRequest(response);
    }
    
    [HttpGet]
    [Route("all")]
    public async Task<IActionResult> GetAllSubscriptionsAsync(bool getAll = false)
    {
        var response = await _subscriptionService.GetAllSubscriptionsAsync(getAll);
        if (response.IsSuccess)
        {
            return Ok(response);
        }
        
        return BadRequest(response);
    }
    
    [HttpGet]
    [Route("{id:guid}")]
    public async Task<IActionResult> GetSubscriptionAsync(Guid id)
    {
        var response = await _subscriptionService.GetSubscriptionAsync(id);
        if (response.IsSuccess)
        {
            return Ok(response);
        }
        return NotFound(response);
    }

    [HttpPut]
    [Route("update/{id:guid}")]
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