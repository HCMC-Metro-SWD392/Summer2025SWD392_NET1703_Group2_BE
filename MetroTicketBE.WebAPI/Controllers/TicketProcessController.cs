using MetroTicketBE.Application.IService;
using MetroTicketBE.Domain.DTO.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MetroTicketBE.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TicketProcessController : ControllerBase
    {
        private readonly ITicketProcessService _ticketProcessService;
        public TicketProcessController(ITicketProcessService ticketProcessService)
        {
            _ticketProcessService = ticketProcessService ?? throw new ArgumentNullException(nameof(ticketProcessService));
        }

        [HttpGet]
        [Route("GetAllTicketProcessByTicketId/{ticketId:guid}")]
        [Authorize]
        public async Task<ActionResult<ResponseDTO>> GetAllTicketProcessByTicketId(Guid ticketId)
        {
            var response = await _ticketProcessService.GetAllTicketProcessByTicketId(ticketId);
            return StatusCode(response.StatusCode, response);
        }
    }
}
