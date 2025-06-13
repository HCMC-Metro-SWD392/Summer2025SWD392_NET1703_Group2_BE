using MetroTicketBE.Application.IService;
using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.DTO.FareRule;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MetroTicketBE.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FareRuleController : ControllerBase
    {
        private readonly IFareRuleService _fareRuleService;
        public FareRuleController(IFareRuleService fareRuleService)
        {
            _fareRuleService = fareRuleService ?? throw new ArgumentNullException(nameof(fareRuleService));
        }

        [HttpPost]
        [Route("create-fare-rule")]
        public async Task<ActionResult<ResponseDTO>> CreateFareRule([FromBody] CreateFareRuleDTO createFareRuleDTO)
        {
            var response = await _fareRuleService.CreateFareRule(createFareRuleDTO);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet]
        [Route("fare-rules/all")]
        public async Task<ActionResult<ResponseDTO>> GetAll
            (
            [FromQuery] string? sortBy,
            [FromQuery] bool? isAcsending,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10
            )
        {
            var response = await _fareRuleService.GetAll(User, sortBy, isAcsending, pageNumber, pageSize);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut]
        [Route("update-fare-rule")]
        public async Task<ActionResult<ResponseDTO>> UpdateFareRule([FromBody] UpdateFareRuleDTO updateFareRuleDTO)
        {
            var response = await _fareRuleService.UpdateFareRule(updateFareRuleDTO);
            return StatusCode(response.StatusCode, response);

        }
    }
}
