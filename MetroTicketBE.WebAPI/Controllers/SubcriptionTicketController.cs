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
    
    
}