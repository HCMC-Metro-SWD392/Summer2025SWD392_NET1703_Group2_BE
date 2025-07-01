using MetroTicketBE.Application.IService;
using MetroTicketBE.Domain.Constants;
using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.DTO.Customer;
using MetroTicketBE.Domain.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MetroTicketBE.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController: ControllerBase
{
    private readonly IUserService _userService;
    public UserController(IUserService userService)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    }
    
    [HttpGet("{userId}")]
    [Authorize]
    public async Task<ActionResult<ResponseDTO>> GetUserByIdAsync(string userId)
    {
        ResponseDTO response = await _userService.GetUserByIdAsync(userId);
        
        if (!response.IsSuccess)
        {
            return NotFound(response);
        }
        
        return Ok(response);
    }
    
    [HttpPut("{userId}")]
    [Authorize]
    public async Task<ActionResult<ResponseDTO>> UpdateUserAsync(string userId, [FromBody] UpdateUserDTO userDTO)
    {
        ResponseDTO response = await _userService.UpdateUserAsync(userId, userDTO);
        
        if (!response.IsSuccess)
        {
            return NotFound(response);
        }
        
        return Ok(response);
    }
    
}