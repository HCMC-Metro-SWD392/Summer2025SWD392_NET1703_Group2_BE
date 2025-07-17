using MetroTicketBE.Application.IService;
using MetroTicketBE.Domain.Constants;
using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.DTO.News;
using MetroTicketBE.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MetroTicketBE.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        private readonly INewsService newsService;
        public NewsController(INewsService newsService)
        {
            this.newsService = newsService ?? throw new ArgumentNullException(nameof(newsService));
        }

        [HttpPost]
        [Route("create-news")]
        [Authorize(Roles = StaticUserRole.Staff)]
        public async Task<ActionResult<ResponseDTO>> CreateNews([FromForm] CreateNewsDTO createNewsDTO)
        {
            if (createNewsDTO == null)
            {
                return BadRequest("Invalid news data.");
            }
            var response = await newsService.CreateNews(User, createNewsDTO);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet]
        [Route("get-all-news-for-manager")]
        [Authorize(Roles = StaticUserRole.Manager)]
        public async Task<ActionResult<ResponseDTO>> GetAllNewsForManager(
            [FromQuery] string? filterOn,
            [FromQuery] string? filerQuery,
            [FromQuery] NewsStatus status,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var response = await newsService.GetAllNewsListForManager(filterOn, filerQuery, status, pageNumber, pageSize);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet]
        [Route("get-news-by-id/{newsId}")]
        [Authorize(Roles = StaticUserRole.StaffManagerAdmin)]
        public async Task<ActionResult<ResponseDTO>> GetNewsById(Guid newsId)
        {
            if (newsId == Guid.Empty)
            {
                return BadRequest("Invalid news ID.");
            }
            var response = await newsService.GetNewsById(newsId);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut]
        [Route("update-news/{newsId}")]
        [Authorize(Roles = StaticUserRole.Staff)]
        public async Task<ActionResult<ResponseDTO>> UpdateNews(Guid newsId, [FromForm] UpdateNewsDTO updateNewsDTO)
        {
            if (newsId == Guid.Empty || updateNewsDTO == null)
            {
                return BadRequest("Invalid news ID or data.");
            }
            var response = await newsService.UpdateNews(User, newsId, updateNewsDTO);
            return StatusCode(response.StatusCode, response);
        }

        [HttpDelete]
        [Route("delete-news/{newsId}")]
        [Authorize(Roles = StaticUserRole.ManagerAdmin)]
        public async Task<ActionResult<ResponseDTO>> DeleteNews(Guid newsId)
        {
            if (newsId == Guid.Empty)
            {
                return BadRequest("Invalid news ID.");
            }
            var response = await newsService.DeleteNews(newsId);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet]
        [Route("get-all-news-for-user")]
        [AllowAnonymous]
        public async Task<ActionResult<ResponseDTO>> GetAllNewsForUser(
            [FromQuery] string? filterOn,
            [FromQuery] string? filerQuery,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var response = await newsService.GetAllNewsListForUser(filterOn, filerQuery, pageNumber, pageSize);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet]
        [Route("get-all-news-for-staff")]
        [Authorize(Roles = StaticUserRole.Staff)]
        public async Task<ActionResult<ResponseDTO>> GetAllNewsForStaff(
            [FromQuery] string? filterOn,
            [FromQuery] string? filerQuery,
            [FromQuery] NewsStatus status,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var response = await newsService.GetAllNewsListForStaff(User, filterOn, filerQuery, status, pageNumber, pageSize);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut]
        [Route("change-news-status/{newsId}")]
        [Authorize(Roles = StaticUserRole.Manager)]
        public async Task<ActionResult<ResponseDTO>> ChangeNewsStatus(Guid newsId, [FromForm] ChangeStatusDTO changeStatusDTO)
        {
            if (newsId == Guid.Empty || changeStatusDTO == null)
            {
                return BadRequest("Invalid news ID or data.");
            }
            var response = await newsService.ChangeNewsStatus(newsId, changeStatusDTO);
            return StatusCode(response.StatusCode, response);
        }
    }
}
