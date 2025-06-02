using MetroTicketBE.Application.IService;
using MetroTicketBE.Domain.DTO.MetroLineStation;
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
        public async Task<IActionResult> CreateMetroLineStation([FromBody] CreateMetroLineStationDTO createMetroLineStationDTO)
        {
            var response = await _metroLineStationService.CreateMetroLineStation(createMetroLineStationDTO);
            return StatusCode(response.StatusCode, response);
        }
    }
}
