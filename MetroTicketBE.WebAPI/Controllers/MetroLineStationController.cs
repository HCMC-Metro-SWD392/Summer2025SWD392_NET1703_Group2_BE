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
        [Authorize(Roles = StaticUserRole.Admin)]
        public async Task<ActionResult<ResponseDTO>> GetStationByMetroLineIdAsync(Guid metroLineId)
        {
            var response = await _metroLineStationService.GetStationByMetroLineIdAsync(metroLineId);
            return StatusCode(response.StatusCode, response);
        }
    }
}
