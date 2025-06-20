using MetroTicketBE.Application.IService;
using MetroTicketBE.Domain.Constants;
using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.DTO.FormRequest;
using MetroTicketBE.Domain.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MetroTicketBE.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FormRequestController : ControllerBase
    {
        private readonly IFormRequestService _formRequestService;
        private readonly IS3Service s3Service;
        public FormRequestController(IFormRequestService formRequestService, IS3Service s3Service)
        {
            _formRequestService = formRequestService ?? throw new ArgumentNullException(nameof(formRequestService));
            this.s3Service = s3Service ?? throw new ArgumentNullException(nameof(s3Service));
        }

        [HttpPost]
        [Route("create-form-request")]
        public async Task<ActionResult<ResponseDTO>> CreateFormRequest([FromForm] CreateFormRequestDTO createFormRequestDTO)
        {
            var response = await _formRequestService.SendFormRequest(User, createFormRequestDTO);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost]
        [Route("upload-file-url-form-request")]
        [Authorize]
        public ActionResult<ResponseDTO> GenerateUploadFormRequestUrl(PreSignedUploadDTO preSignedUploadDTO)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(new ResponseDTO
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "User ID is required."
                });
            }
            var objectKey = $"form-request-picture/{userId}/{Guid.NewGuid()}_{preSignedUploadDTO.FileName}";
            var response = s3Service.GenerateUploadUrl(objectKey, preSignedUploadDTO.ContentType);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet]
        [Route("download-file-url-form-request")]
        [Authorize]
        public async Task<ActionResult<ResponseDTO>> GetFormRequest()
        {
            var response = await _formRequestService.GetFormRequest(User);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet]
        [Route("get-all-form-requests")]
        //[Authorize(Roles = StaticUserRole.StaffManagerAdmin)]
        public async Task<ActionResult<ResponseDTO>> GetAllFormRequests(
           [FromQuery] string sortBy = "createdAt",
           [FromQuery] FormStatus formStatus = FormStatus.Pending,
           [FromQuery] bool? isAcsending = true,
           [FromQuery] int pageNumber = 1,
           [FromQuery] int pageSize = 10)
        {
            var response = await _formRequestService.GetAll(sortBy, formStatus, isAcsending, pageNumber, pageSize);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet]
        [Route("get-all-form-attachments")]
        public async Task<ActionResult<ResponseDTO>> GetAllFormAttachments([FromQuery] Guid formRequestId)
        {
            var response = await _formRequestService.GetAllFormAttachment(formRequestId);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut]
        [Route("change-form-request-status/{formRequestId:Guid}")]
        public async Task<ActionResult<ResponseDTO>> ChangeFormRequestStatus([FromRoute] Guid formRequestId, [FromBody] ChangeFormStatusDTO changeFormStatusDTO)
        {
            var response = await _formRequestService.ChangeFormRequestStatus(User, formRequestId, changeFormStatusDTO);
            return StatusCode(response.StatusCode, response);
        }
    }
}
