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
    
    [HttpPut("{customerId:guid}")]
    public async Task<ActionResult<ResponseDTO>> UpdateCustomerAsync(Guid customerId, [FromBody] UpdateCustomerDTO updateCustomerDTO)
    {
        ResponseDTO response = await _customerService.UpdateCustomerAsync(customerId, updateCustomerDTO);
        
        if (!response.IsSuccess)
        {
            return NotFound(response);
        }
        
        return Ok(response);
    }
    
    [HttpGet("user/{userId}")]
    public async Task<ActionResult<ResponseDTO>> GetCustomerByUserIdAsync(string userId)
    {
        ResponseDTO response = await _customerService.GetCustomerByUserIdAsync(userId);
        
        if (!response.IsSuccess)
        {
            return NotFound(response);
        }
        
        return Ok(response);
    }
    
    [HttpGet("email/{email}")]
    public async Task<ActionResult<ResponseDTO>> GetCustomerByEmailAsync(string email)
    {
        ResponseDTO response = await _customerService.GetCustomerByEmailAsync(email);
        
        if (!response.IsSuccess)
        {
            return NotFound(response);
        }
        
        return Ok(response);
    }
}