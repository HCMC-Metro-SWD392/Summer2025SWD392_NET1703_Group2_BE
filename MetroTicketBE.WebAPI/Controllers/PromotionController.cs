using MetroTicketBE.Application.IService;
using MetroTicketBE.Domain.Constants;
using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.DTO.Promotion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MetroTicketBE.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PromotionController : ControllerBase
    {
        private readonly IPromotionService _promotionService;
        public PromotionController(IPromotionService promotionService)
        {
            _promotionService = promotionService;
        }
        [HttpPost]
        [Route("create-promotion")]
        [Authorize(Roles = StaticUserRole.ManagerAdmin)]
        public async Task<ActionResult<ResponseDTO>> CreatePromotion([FromBody] CreatePromotionDTO createPromotionDTO)
        {
            var response = await _promotionService.CreatePromotion(createPromotionDTO);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPatch]
        [Route("update-promotion")]
        [Authorize(Roles = StaticUserRole.ManagerAdmin)]
        public async Task<ActionResult<ResponseDTO>> UpdatePromotion([FromBody] UpdatePromotionDTO updatePromotionDTO)
        {
            var response = await _promotionService.UpdatePromotion(updatePromotionDTO);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet]
        [Route("get-all-promotions")]
        [Authorize(Roles = StaticUserRole.ManagerAdmin)]
        public async Task<ActionResult<ResponseDTO>> GetAllPromotions(
            [FromQuery] string? filterOn,
            [FromQuery] string? filterQuery,
            [FromQuery] string? sortBy,
            [FromQuery] bool? isAcsending,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var response = await _promotionService.GetAll(User, filterOn, filterQuery, sortBy, isAcsending, pageNumber, pageSize);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet]
        [Route("get-promotion/{promotionId}")]
        [Authorize(Roles = StaticUserRole.ManagerAdmin)]
        public async Task<ActionResult<ResponseDTO>> GetPromotionById([FromRoute] Guid promotionId)
        {
            var response = await _promotionService.GetPromotionById(promotionId);
            return StatusCode(response.StatusCode, response);
        }

        [HttpDelete]
        [Route("delete-promotion/{promotionId}")]
        [Authorize(Roles = StaticUserRole.ManagerAdmin)]
        public async Task<ActionResult<ResponseDTO>> DeletePromotion([FromRoute] Guid promotionId)
        {
            var response = await _promotionService.DeletePromotion(promotionId);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet]
        [Route("get-promotion-by-code/{code}")]
        [Authorize]
        public async Task<ActionResult<ResponseDTO>> GetPromotionByCode([FromRoute] string code)
        {
            var response = await _promotionService.GetPromotionByCode(code);
            return StatusCode(response.StatusCode, response);
        }
    }
}
