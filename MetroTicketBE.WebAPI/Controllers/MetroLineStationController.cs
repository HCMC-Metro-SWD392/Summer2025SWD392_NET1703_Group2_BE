using MetroTicketBE.Application.IService;
using MetroTicketBE.Domain.Constants;
using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.DTO.MetroLineStation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MetroTicketBE.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MetroLineStationController : ControllerBase
    {
        private readonly IMetroLineStationService _metroLineStationService;
        public MetroLineStationController(IMetroLineStationService metroLineStationService)
        {
            _metroLineStationService = metroLineStationService ?? throw new ArgumentNullException(nameof(metroLineStationService));
        }

        [HttpPost]
        [Route("create-metro-line-station")]
        [Authorize(Roles = StaticUserRole.Admin)]
        public async Task<ActionResult<ResponseDTO>> CreateMetroLineStation([FromBody] CreateMetroLineStationDTO createMetroLineStationDTO)
        {
            var response = await _metroLineStationService.CreateMetroLineStation(createMetroLineStationDTO);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet]
        [Route("get-station-by-metro-line-id/{metroLineId}")]
        //[Authorize(Roles = StaticUserRole.Admin)]
        public async Task<ActionResult<ResponseDTO>> GetStationByMetroLineIdAsync(Guid metroLineId, [FromQuery] bool? isActive = null)
        {
            var response = await _metroLineStationService.GetStationByMetroLineIdAsync(metroLineId, isActive);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPatch]
        [Route("remove-metroline-station/{metroLineStationId}")]
        [Authorize(Roles = StaticUserRole.Admin)]
        public async Task<ActionResult<ResponseDTO>> SetActiveMetroLineStation(Guid metroLineStationId)
        {
            var response = await _metroLineStationService.RemoveMetroLineStation(metroLineStationId);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut]
        [Route("update-metroline-station/{metroLineStationId}")]
        [Authorize(Roles = StaticUserRole.Admin)]
        public async Task<ActionResult<ResponseDTO>> UpdateMetroLineStation(Guid metroLineStationId,
            [FromBody] UpdateMetroLineStationDTO updateDTO)
        {
            var response = await _metroLineStationService.UpdateMetroLineStation(metroLineStationId, updateDTO);
            return StatusCode(response.StatusCode, response);
        }
    }
}
