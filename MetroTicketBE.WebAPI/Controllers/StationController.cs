using MetroTicketBE.Application.IService;
using MetroTicketBE.Application.Service;
using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.DTO.Station;
using Microsoft.AspNetCore.Http;
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
        public async Task<ActionResult<ResponseDTO>> CreateStation([FromBody] CreateStationDTO createStationDTO)
        {
            var response = await _stationService.CreateStation(createStationDTO);
            return StatusCode(response.StatusCode, response);
        }
        

        [HttpPut]
        [Route("update-station/{stationId}")]
        public async Task<ActionResult<ResponseDTO>> UpdateStation(Guid stationId,
            [FromBody] UpdateStationDTO updateStationDTO)
        {
            var response = await _stationService.UpdateStation(stationId, updateStationDTO);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet]
        [Route("get-all-stations")]
        public async Task<ActionResult<ResponseDTO>> GetAllStations(bool? isAscending)
        {
            var response = await _stationService.GetAllStations(isAscending);
            return StatusCode(response.StatusCode, response);
        }
        
        [HttpGet]
        [Route("get-station-by-id/{stationId}")]
        public async Task<ActionResult<ResponseDTO>> GetStationById(Guid stationId)
        {
            var response = await _stationService.GetStationById(stationId);
            return StatusCode(response.StatusCode, response);
        }
    }
}
