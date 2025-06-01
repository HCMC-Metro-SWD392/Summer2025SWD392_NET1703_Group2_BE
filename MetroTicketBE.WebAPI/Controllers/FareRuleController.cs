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

    }
}
