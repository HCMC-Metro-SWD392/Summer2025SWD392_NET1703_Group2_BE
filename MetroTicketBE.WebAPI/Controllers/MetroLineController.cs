using MetroTicketBE.Application.IService;
using MetroTicketBE.Domain.Constants;
using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.DTO.MetroLine;
using MetroTicketBE.Domain.Enums;
using MetroTicketBE.Infrastructure.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MetroTicketBE.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MetroLineController : ControllerBase
    {
        private readonly IMetroLineService _metroLineService;
        public MetroLineController(IMetroLineService metroLineService)
        {
            _metroLineService = metroLineService ?? throw new ArgumentNullException(nameof(metroLineService));
        }

        [HttpPost]
        [Route("create-metro-line")]
        [Authorize(Roles = StaticUserRole.Admin)]
        public async Task<ActionResult<ResponseDTO>> CreateMetroLine([FromBody] CreateMetroLineDTO createMetroLineDTO)
        {
            var response = await _metroLineService.CreateMetroLine(User, createMetroLineDTO);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet]
        [Route("metro-lines/all")]
        [AllowAnonymous]
        public async Task<ActionResult<ResponseDTO>> GetAllMetroLines([FromQuery] bool? isActive = null)
        {
            var response = await _metroLineService.GetAllMetroLines(isActive);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet]
        [Route("metro-line/{metroLineId}")]
        [Authorize(Roles = StaticUserRole.Admin)]
        public async Task<ActionResult<ResponseDTO>> GetMetroLineById(Guid metroLineId)
        {
            var response = await _metroLineService.GetMetroLineById(metroLineId);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut]
        [Route("update-metro-line/{metroLineId}")]
        [Authorize(Roles = StaticUserRole.Admin)]
        public async Task<ActionResult<ResponseDTO>> UpdateMetroLine(Guid metroLineId,
            [FromBody] UpdateMetroLineDTO updateMetroLineDTO)
        {
            var response = await _metroLineService.UpdateMetroLine(User, metroLineId, updateMetroLineDTO);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPatch]
        [Route("set-isActive/{metroLineId:guid}")]
        [Authorize(Roles = StaticUserRole.Admin)]
        public async Task<ActionResult<ResponseDTO>> SetIsActive(Guid metroLineId, [FromQuery] bool isActive)
        {
            var response = await _metroLineService.SetIsActiveMetroLine(User, metroLineId, isActive);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut]
        [Route("change-status/{metroLineId:guid}")]
        [Authorize(Roles = StaticUserRole.ManagerAdmin)]
        public async Task<ActionResult<ResponseDTO>> ChangeMetroLineStatus(Guid metroLineId, [FromQuery] MetroLineStatus metroLineStatus)
        {
            var response = await _metroLineService.ChangeMetroLineStatus(metroLineId, metroLineStatus);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet]
        [Route("check-metro-line-error-in-path")]
        [AllowAnonymous]
        public async Task<ActionResult<ResponseDTO>> CheckMetroLineErrorInPath([FromQuery] Guid stationStartId, [FromQuery] Guid stationEndId)
        {
            var response = await _metroLineService.CheckMetroLineErrorInPath(stationStartId, stationEndId);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet]
        [Route("check-metro-line-error-in-path-V2")]
        [AllowAnonymous]
        public async Task<ActionResult<ResponseDTO>> CheckMetroLineErrorInPathV2(Guid ticketId)
        {
            var response = await _metroLineService.CheckMetroLineErrorInPathV2(ticketId);
            return StatusCode(response.StatusCode, response);
        }
    }
}
