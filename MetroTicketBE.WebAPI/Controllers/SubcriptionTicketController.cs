using MetroTicketBE.Application.IService;
using MetroTicketBE.Domain.DTO.Auth;
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
    
    [HttpPost("{customerId:guid}/subscribe")]
    public async Task<ActionResult<ResponseDTO>> SubscribeToTicketAsync(Guid customerId)
    {
        ResponseDTO response = await _subscriptionService.AddSubscriptionAsync(customerId);
        if (!response.IsSuccess)
        {
            return StatusCode(response.StatusCode, response);
        }
        return Ok(response);
    }
}