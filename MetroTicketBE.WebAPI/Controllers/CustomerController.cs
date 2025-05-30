using MetroTicketBE.Application.IService;
using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.DTO.Customer;
using Microsoft.AspNetCore.Mvc;

namespace MetroTicketBE.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomerController: ControllerBase
{
    private readonly ICustomerService _customerService;
    
    public CustomerController(ICustomerService customerService)
    {
        _customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));
    }
    
    [HttpGet("{customerId:guid}")]
    public async Task<ActionResult<ResponseDTO>> GetCustomerByIdAsync(Guid customerId)
    {
        ResponseDTO response = await _customerService.GetCustomerByIdAsync(customerId);
        
        if (!response.IsSuccess)
        {
            return NotFound(response);
        }
        
        return Ok(response);
    }
    
}