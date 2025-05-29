using MetroTicketBE.Application.IService;
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
    public async Task<ActionResult<CustomerResponseDTO>> GetCustomerByIdAsync(Guid customerId)
    {
        var customer = await _customerService.GetCustomerByIdAsync(customerId);
        
        if (customer == null)
        {
            return NotFound(new { Message = "Customer not found" });
        }
        
        return Ok(customer);
    }
}