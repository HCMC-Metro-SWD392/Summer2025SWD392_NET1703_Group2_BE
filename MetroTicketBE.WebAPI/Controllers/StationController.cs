using MetroTicketBE.Application.IService;
using MetroTicketBE.Domain.Constants;
using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.DTO.Station;
using MetroTicketBE.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MetroTicketBE.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StationController : ControllerBase
    {
        private readonly IStationService _stationService;
        public StationController(IStationService stationService)
        {
            _stationService = stationService ?? throw new ArgumentNullException(nameof(stationService));
        }
        [HttpPost]
        [Route("create-station")]
        [Authorize(Roles = StaticUserRole.ManagerAdmin)]
        public async Task<ActionResult<ResponseDTO>> CreateStation([FromBody] CreateStationDTO createStationDTO)
        {
            var response = await _stationService.CreateStation(User, createStationDTO);
            return StatusCode(response.StatusCode, response);
        }


        [HttpPut]
        [Route("update-station/{stationId}")]
        [Authorize(Roles = StaticUserRole.ManagerAdmin)]
        public async Task<ActionResult<ResponseDTO>> UpdateStation(Guid stationId,
            [FromBody] UpdateStationDTO updateStationDTO)
        {
            var response = await _stationService.UpdateStation(User, stationId, updateStationDTO);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet]
        [Route("get-all-stations")]
        [Authorize]
        public async Task<ActionResult<ResponseDTO>> GetAllStations(bool? isAscending, int pageNumber, int pageSize, bool? isActive = null)
        {
            var response = await _stationService.GetAllStations(isAscending, pageNumber, pageSize, isActive);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet]
        [Route("get-station-by-id/{stationId}")]
        [Authorize(Roles = StaticUserRole.StaffManagerAdmin)]
        public async Task<ActionResult<ResponseDTO>> GetStationById(Guid stationId)
        {
            var response = await _stationService.GetStationById(stationId);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet]
        [Route("search-stations-by-name")]
        [Authorize]
        public async Task<ActionResult<ResponseDTO>> SearchStationsByName([FromQuery] string? name, bool? isActive = null)
        {
            var response = await _stationService.SearchStationsByName(name, isActive);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPatch]
        [Route("set-isActive/{stationId:guid}")]
        [Authorize(Roles = StaticUserRole.Admin)]
        public async Task<ActionResult<ResponseDTO>> SetIsActive(Guid stationId, [FromQuery] bool isActive)
        {
            var response = await _stationService.SetIsActiveStation(User, stationId, isActive);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet]
        [Route("search-ticket-road")]
        [AllowAnonymous]
        public async Task<ActionResult<ResponseDTO>> SearchTicketRoad([FromQuery] Guid stationStart, [FromQuery] Guid stationEnd, bool? isSideRoad)
        {
            var response = await _stationService.SearchTicketRoad(stationStart, stationEnd, isSideRoad);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet]
        [Route("search-ticket-road-v2")]
        [AllowAnonymous]
        public async Task<ActionResult<ResponseDTO>> SearchTicketRoadV2([FromQuery] Guid ticketId, bool? isSideRoad)
        {
            var response = await _stationService.SearchTicketRoadV2(ticketId, isSideRoad);
            return StatusCode(response.StatusCode, response);
        }
    }
}
