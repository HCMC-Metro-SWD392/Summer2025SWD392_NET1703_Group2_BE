using MetroTicket.Domain.Entities;
using MetroTicketBE.Application.IService;
using MetroTicketBE.Domain.Constants;
using MetroTicketBE.Domain.DTO.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MetroTicketBE.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthController(IAuthService authService, UserManager<ApplicationUser> userManager)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
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

        [HttpPost]
        [Route("sign-in")]
        public async Task<ActionResult<ResponseDTO>> LoginUser([FromBody] LoginDTO loginDTO)
        {
            var response = await _authService.LoginUser(loginDTO);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost]
        [Route("sign-in-by-google")]
        public async Task<ActionResult<ResponseDTO>> LoginUserByGoogle([FromBody] LoginByGoogleDTO loginByGoogleDTO)
        {
            var response = await _authService.LoginUserByGoogle(loginByGoogleDTO);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost]
        [Route("send-verify-email")]
        public async Task<ActionResult<ResponseDTO>> SendVerifyEmail([FromBody] SendVerifyEmailDTO sendVerifyEmailDTO)
        {
            var response = await _authService.SendVerifyEmail(sendVerifyEmailDTO.Email);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost]
        [Route("verify-email")]
        [ActionName("verify-email")]
        public async Task<ActionResult<ResponseDTO>> VerifyEmail([FromQuery] string email, [FromQuery] string token)
        {
            var response = await _authService.VerifyEmail(email, token);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost]
        [Route("logout")]
        public async Task<ActionResult<ResponseDTO>> Logout()
        {
            var response = await _authService.Logout(User);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut]
        [Route("set-staff-role/{email}")]
        [Authorize(Roles = StaticUserRole.Manager)]
        public async Task<ActionResult<ResponseDTO>> SetStaffRole([FromRoute] string email)
        {
            var response = await _authService.SetStaffRole(email);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut]
        [Route("set-manager-role/{email}")]
        [Authorize(Roles = StaticUserRole.Admin)]
        public async Task<ActionResult<ResponseDTO>> SetManagerRole([FromRoute] string email)
        {
            var response = await _authService.SetManagerRole(email);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut]
        [Route("set-admin-role/{email}")]
        [Authorize(Roles = StaticUserRole.Admin)]
        public async Task<ActionResult<ResponseDTO>> SetAdminRole([FromRoute] string email)
        {
            var response = await _authService.SetAdminRole(email);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("create-staff")]
        [Authorize(Roles = StaticUserRole.Manager)]
        public async Task<ActionResult<ResponseDTO>> CreateStaffAsync([FromBody] RegisterCustomerDTO dto)
        {
            ResponseDTO response = await _authService.CreateStaffAsync(dto);

            if (!response.IsSuccess)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("create-manager")]
        [Authorize(Roles = StaticUserRole.Admin)]
        public async Task<ActionResult<ResponseDTO>> CreateManagerAsync([FromBody] RegisterCustomerDTO dto)
        {
            ResponseDTO response = await _authService.CreateManagerAsync(dto);

            if (!response.IsSuccess)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("create-admin")]
        [Authorize(Roles = StaticUserRole.Admin)]
        public async Task<ActionResult<ResponseDTO>> CreateAdminAsync([FromBody] RegisterCustomerDTO dto)
        {
            ResponseDTO response = await _authService.CreateAdminAsync(dto);

            if (!response.IsSuccess)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPut]
        [Route("change-password")]
        [Authorize]
        public async Task<ActionResult<ResponseDTO>> ChangePassword([FromBody] ChangePasswordDTO changePasswordDTO)
        {
            var response = await _authService.ChangPassword(User, changePasswordDTO);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost]
        [Route("send-reset-password-email")]
        public async Task<ActionResult<ResponseDTO>> ResetPassword([FromBody] SendResetPasswordDTO resetPasswordDTO)
        {
            var response = await _authService.SendResetPasswordEmail(resetPasswordDTO);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost]
        [Route("reset-password")]
        public async Task<ActionResult<ResponseDTO>> ResetPassword([FromBody] ResetPasswordDTO resetPasswordDTO)
        {
            var response = await _authService.ResetPassword(resetPasswordDTO);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut]
        [Route("remove-staff/")]
        [Authorize(Roles = StaticUserRole.ManagerAdmin)]
        public async Task<ActionResult<ResponseDTO>> RemoveStaff([FromQuery]string email)
        {
            var response = await _authService.RemoveStaff(email);
            return StatusCode(response.StatusCode, response);
        }
    }
}
