using MetroTicketBE.Application.IService;
using MetroTicketBE.Domain.Constants;
using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.DTO.Customer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MetroTicketBE.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomerController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomerController(ICustomerService customerService)
    {
        _customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));
    }

    [HttpGet("{customerId:guid}")]
    [Authorize(Roles = StaticUserRole.StaffManagerAdmin)]
    public async Task<ActionResult<ResponseDTO>> GetCustomerByIdAsync(Guid customerId)
    {
        ResponseDTO response = await _customerService.GetCustomerByIdAsync(customerId);

        if (!response.IsSuccess)
        {
            return NotFound(response);
        }

        return Ok(response);
    }


    [HttpGet("user/{userId}")]
    [Authorize]
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
    [Authorize(Roles = StaticUserRole.StaffManagerAdmin)]
    public async Task<ActionResult<ResponseDTO>> GetCustomerByEmailAsync(string email)
    {
        ResponseDTO response = await _customerService.GetCustomerByEmailAsync(email);

        if (!response.IsSuccess)
        {
            return NotFound(response);
        }

        return Ok(response);
    }

    [HttpGet]
    [Route("get-all-customers")]
    [Authorize(Roles = StaticUserRole.ManagerAdmin)]
    public async Task<ActionResult<ResponseDTO>> GetAllCustomersAsync(
        [FromQuery] string? filterOn,
        [FromQuery] string? filterQuery,
        [FromQuery] string? sortBy,
        [FromQuery] bool? isAscending,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var response = await _customerService.GetAllCustomersAsync(filterOn, filterQuery, sortBy, isAscending, pageNumber, pageSize);
        return StatusCode(response.StatusCode, response);
    }
}