using MetroTicketBE.Application.IService;
using MetroTicketBE.Domain.DTO.Auth;
using Microsoft.AspNetCore.Mvc;

namespace MetroTicketBE.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        [HttpPost("customer/register")]
        public async Task<ActionResult<ResponseDTO>> RegisterCustomer([FromBody] RegisterCustomerDTO registerCustomerDTO)
        {
            var response = new ResponseDTO();
            if (ModelState.IsValid is false)
            {
                response.Message = "Dữ liệu không hợp lệ";
                response.IsSuccess = false;
                response.Result = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(response);
            }

            try
            {
                var result = await _authService.RegisterCustomer(registerCustomerDTO);
                if (result.IsSuccess is true)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception e)
            {
                response.IsSuccess = false;
                response.Result = e.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }
    }
}
