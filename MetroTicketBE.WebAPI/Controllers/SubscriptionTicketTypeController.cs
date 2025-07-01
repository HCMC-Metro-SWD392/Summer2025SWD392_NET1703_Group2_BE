using MetroTicketBE.Application.IService;
using MetroTicketBE.Domain.Constants;
using MetroTicketBE.Domain.DTO.SubscriptionTicketType;
using MetroTicketBE.Infrastructure.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MetroTicketBE.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubscriptionTicketTypeController: ControllerBase
{
    private readonly ISubscriptionTicketTypeService _subscriptionTicketTypeService;
    public SubscriptionTicketTypeController(ISubscriptionTicketTypeService subscriptionTicketTypeService)
    {
        _subscriptionTicketTypeService = subscriptionTicketTypeService ?? throw new ArgumentNullException(nameof(subscriptionTicketTypeService));
    }
    
    [HttpPost]
    [Route("create-subscription-ticket-type")]
    [Authorize(Roles = StaticUserRole.ManagerAdmin)]
    public async Task<IActionResult> CreateSubscriptionTicketTypeAsync([FromBody] CreateSubscriptionTicketTypeDTO subscriptionTicketType)
    {
        var response = await _subscriptionTicketTypeService.CreateAsync(subscriptionTicketType);
        if (response.IsSuccess)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }
    
    [HttpGet]
    [Route("all")]
    [Authorize(Roles = StaticUserRole.ManagerAdmin)]
    public async Task<IActionResult> GetAllSubscriptionTicketTypesAsync()
    {
        var response = await _subscriptionTicketTypeService.GetAllAsync();
        if (response.IsSuccess)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }
    [HttpGet]
    [Route("{id:guid}")]
    [Authorize(Roles = StaticUserRole.ManagerAdmin)]
    public async Task<IActionResult> GetSubscriptionTicketTypeAsync(Guid id)
    {
        var response = await _subscriptionTicketTypeService.GetByIdAsync(id);
        if (response.IsSuccess)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }
    [HttpGet]
    [Route("name/{name}")]
    [Authorize]
    public async Task<IActionResult> GetSubscriptionTicketTypeByNameAsync(string name)
    {
        var response = await _subscriptionTicketTypeService.GetByNameAsync(name);
        if (response.IsSuccess)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }
    
}