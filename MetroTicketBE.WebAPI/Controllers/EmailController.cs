using MetroTicketBE.Application.IService;
using MetroTicketBE.Domain.Constants;
using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.DTO.Email;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MetroTicketBE.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;
        public EmailController(IEmailService emailService)
        {
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        }

        [HttpPost]
        [Route("create-eamil-template")]
        [Authorize(Roles = StaticUserRole.Admin)]
        public async Task<ActionResult<ResponseDTO>> CreateEmailTemplate([FromBody] CreateEmailTemplateDTO createEmailTemplateDTO)
        {
            var response = await _emailService.CreateEmailTemplate(User, createEmailTemplateDTO);
            return StatusCode(response.StatusCode, response);

        }

        [HttpPut]
        [Route("update-email-template/{templateId}")]
        [Authorize(Roles = StaticUserRole.Admin)]
        public async Task<ActionResult<ResponseDTO>> UpdateEmailTemplate([FromRoute] Guid templateId, [FromBody] UpdateEmailTemplateDTO updateEmailTemplateDTO)
        {
            var response = await _emailService.UpdateEmailTemplate(User, templateId, updateEmailTemplateDTO);
            return StatusCode(response.StatusCode, response);
        }
    }
}
