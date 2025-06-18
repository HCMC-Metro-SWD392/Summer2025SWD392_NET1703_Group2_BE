using MetroTicketBE.Application.IService;
using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.DTO.MetroLine;
using MetroTicketBE.Infrastructure.IRepository;
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

        public async Task<ActionResult<ResponseDTO>> CreateMetroLine([FromBody] CreateMetroLineDTO createMetroLineDTO)
        {
            var response = await _metroLineService.CreateMetroLine(createMetroLineDTO);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet]
        [Route("metro-lines/all")]
        public async Task<ActionResult<ResponseDTO>> GetAllMetroLines()
        {
            var response = await _metroLineService.GetAllMetroLines();
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet]
        [Route("metro-line/{metroLineId}")]
        public async Task<ActionResult<ResponseDTO>> GetMetroLineById(Guid metroLineId)
        {
            var response = await _metroLineService.GetMetroLineById(metroLineId);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut]
        [Route("update-metro-line/{metroLineId}")]
        public async Task<ActionResult<ResponseDTO>> UpdateMetroLine(Guid metroLineId,
            [FromBody] UpdateMetroLineDTO updateMetroLineDTO)
        {
            var response = await _metroLineService.UpdateMetroLine(metroLineId, updateMetroLineDTO);
            return StatusCode(response.StatusCode, response);
        }
    }
}
